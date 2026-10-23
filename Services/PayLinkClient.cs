using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Repositories;

namespace Suvarna.DGMS.Api.Services
{
    /// <summary>
    /// NEW: server-side PayLink client (authtoken, pushtrans, transstatus).
    /// The PayLink user / password / token never reach the browser.
    /// Every call is written to DGMS.DGM_POS_API_LOG (password, token and card number masked)
    /// and to the API log file with the request's TraceIdentifier.
    /// </summary>
    public class PayLinkClient
    {
        public const string HttpClientName = "PayLink";
        private const string TokenCacheKey = "PayLink:Token";

        // one token per PayLink user / URL (locations can have their own credentials in System Parameters)
        private string TokenKey => TokenCacheKey + ":" + _options.UserName + "|" + _options.AuthUrl;

        private readonly IHttpClientFactory _httpFactory;
        private readonly PayLinkSettings _settings;
        private PayLinkOptions _options => _settings.Current;
        private readonly IMemoryCache _cache;
        private readonly PosPaymentRepository _repo;
        private readonly ILogger<PayLinkClient> _log;
        private readonly IHttpContextAccessor _http;

        private static readonly SemaphoreSlim TokenLock = new(1, 1);

        public PayLinkClient(
            IHttpClientFactory httpFactory,
            PayLinkSettings settings,
            IMemoryCache cache,
            PosPaymentRepository repo,
            ILogger<PayLinkClient> log,
            IHttpContextAccessor http)
        {
            _httpFactory = httpFactory;
            _settings = settings;
            _cache = cache;
            _repo = repo;
            _log = log;
            _http = http;
        }

        // =====================================================
        // RESULT OF ONE CALL
        // =====================================================

        public class CallResult
        {
            public int? HttpStatus { get; set; }            // null = no answer (timeout / network)
            public JsonObject? Body { get; set; }
            public string? Error { get; set; }

            public bool Answered => HttpStatus != null;
            public bool HttpOk => HttpStatus is >= 200 and < 300;

            // PayLink success = code 200 and status "true"
            public bool PayLinkOk =>
                HttpOk && Body != null && ReadInt(Body, "code") == 200 &&
                string.Equals(ReadString(Body, "status"), "true", StringComparison.OrdinalIgnoreCase);

            public string? Message => Body == null ? Error : (ReadString(Body, "message") ?? Error);

            public string? Get(string name) => Body == null ? null : ReadString(Body, name);
        }

        public class LogContext
        {
            public long? PosTxnId { get; set; }
            public string? TxnNo { get; set; }
            public string? SessionId { get; set; }
        }

        // =====================================================
        // PUBLIC CALLS
        // =====================================================

        /// <summary>pushtrans: sends the amount to the POS machine</summary>
        public Task<CallResult> PushAsync(object payload, LogContext ctx, CancellationToken ct = default)
            => CallWithTokenAsync("PUSH", _options.PushUrl, payload, ctx, ct);

        /// <summary>transstatus: result of a pushed transaction</summary>
        public Task<CallResult> StatusAsync(object payload, LogContext ctx, CancellationToken ct = default)
            => CallWithTokenAsync("STATUS", _options.StatusUrl, payload, ctx, ct);

        /// <summary>canceltrans: takes a pushed transaction off the machine</summary>
        public Task<CallResult> CancelAsync(object payload, LogContext ctx, CancellationToken ct = default)
            => CallWithTokenAsync("CANCEL", _options.CancelUrl, payload, ctx, ct);

        // =====================================================
        // TOKEN
        // =====================================================

        private async Task<(string? token, CallResult auth)> GetTokenAsync(LogContext ctx, bool forceNew, CancellationToken ct)
        {
            if (!forceNew && _cache.TryGetValue(TokenKey, out string? cached) && !string.IsNullOrEmpty(cached))
                return (cached, new CallResult { HttpStatus = 200 });

            await TokenLock.WaitAsync(ct);
            try
            {
                if (!forceNew && _cache.TryGetValue(TokenKey, out cached) && !string.IsNullOrEmpty(cached))
                    return (cached, new CallResult { HttpStatus = 200 });

                var auth = await SendAsync("AUTH", _options.AuthUrl,
                    new { UserName = _options.UserName, Password = _options.Password }, null, ctx, ct);

                var token = auth.Get("token");
                if (!auth.PayLinkOk || string.IsNullOrWhiteSpace(token))
                {
                    _cache.Remove(TokenKey);
                    return (null, auth);
                }

                token = System.Text.RegularExpressions.Regex.Replace(token, @"^Bearer\s+", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

                if (_options.TokenCacheMinutes > 0)
                    _cache.Set(TokenKey, token, TimeSpan.FromMinutes(_options.TokenCacheMinutes));

                return (token, auth);
            }
            finally
            {
                TokenLock.Release();
            }
        }

        private async Task<CallResult> CallWithTokenAsync(string apiName, string url, object payload, LogContext ctx, CancellationToken ct)
        {
            var (token, auth) = await GetTokenAsync(ctx, forceNew: false, ct);
            if (token == null)
            {
                return new CallResult
                {
                    HttpStatus = null,
                    Error = "PayLink authentication failed: " + (auth.Message ?? "no token"),
                    Body = null
                };
            }

            var result = await SendAsync(apiName, url, payload, token, ctx, ct);

            // An expired cached token is rejected with 401 before PayLink does anything,
            // so one retry with a new token is safe (the amount was not sent to the machine).
            if (result.HttpStatus == (int)HttpStatusCode.Unauthorized)
            {
                (token, auth) = await GetTokenAsync(ctx, forceNew: true, ct);
                if (token == null)
                    return new CallResult { HttpStatus = null, Error = "PayLink authentication failed: " + (auth.Message ?? "no token") };
                result = await SendAsync(apiName, url, payload, token, ctx, ct);
            }

            return result;
        }

        // =====================================================
        // HTTP + LOG
        // =====================================================

        private async Task<CallResult> SendAsync(string apiName, string url, object payload, string? bearer, LogContext ctx, CancellationToken ct)
        {
            var requestJson = JsonSerializer.Serialize(payload);
            var correlationId = _http.HttpContext?.TraceIdentifier;
            var userId = int.TryParse(_http.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var u) ? u : (int?)null;

            var sw = Stopwatch.StartNew();
            var result = new CallResult();
            string? responseText = null;

            try
            {
                var client = _httpFactory.CreateClient(HttpClientName);
                client.Timeout = TimeSpan.FromSeconds(Math.Max(5, _options.TimeoutSeconds));

                using var req = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
                };
                if (!string.IsNullOrEmpty(bearer))
                    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

                using var resp = await client.SendAsync(req, ct);
                responseText = await resp.Content.ReadAsStringAsync(ct);
                result.HttpStatus = (int)resp.StatusCode;

                try
                {
                    result.Body = string.IsNullOrWhiteSpace(responseText) ? null : JsonNode.Parse(responseText) as JsonObject;
                }
                catch (JsonException)
                {
                    result.Error = "PayLink returned a response that is not JSON.";
                }

                if (!result.HttpOk && result.Error == null)
                    result.Error = $"PayLink HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}";
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested)
            {
                result.Error = $"PayLink did not answer in {_options.TimeoutSeconds} seconds.";
            }
            catch (HttpRequestException ex)
            {
                result.Error = "PayLink could not be reached: " + ex.Message;
            }
            finally
            {
                sw.Stop();
            }

            var maskedRequest = PayLinkMask.Json(requestJson);
            var maskedResponse = responseText == null ? null : PayLinkMask.Json(responseText);

            // DB log keeps the complete request / response (every field, full text);
            // only the PayLink password, the token and a full card number are hidden
            var fullRequest = PayLinkMask.Full(requestJson);
            var fullResponse = responseText == null ? null : PayLinkMask.Full(responseText);
            var ok = apiName == "AUTH" ? result.PayLinkOk && !string.IsNullOrEmpty(result.Get("token")) : result.PayLinkOk;

            _log.LogInformation(
                "PayLink {Api} {Url} txn={TxnNo} http={Http} ok={Ok} {Ms}ms corr={Corr} req={Req} resp={Resp} err={Err}",
                apiName, url, ctx.TxnNo, result.HttpStatus, ok, sw.ElapsedMilliseconds, correlationId,
                maskedRequest, maskedResponse, result.Error);

            var logged = await _repo.InsertLogAsync(ctx.PosTxnId, ctx.TxnNo, apiName, "POST", url,
                fullRequest, fullResponse, result.HttpStatus, ok, result.Error,
                (int)Math.Min(int.MaxValue, sw.ElapsedMilliseconds), correlationId, ctx.SessionId, userId);

            if (!logged)
                _log.LogError("PayLink {Api} txn={TxnNo}: DB log row could not be written (see file log above)", apiName, ctx.TxnNo);

            return result;
        }

        // =====================================================
        // JSON HELPERS (PayLink sends some numbers / booleans as text)
        // =====================================================

        public static string? ReadString(JsonObject obj, string name)
        {
            var node = FindProperty(obj, name);
            if (node == null) return null;
            if (node is JsonValue v)
            {
                if (v.TryGetValue<string>(out var s)) return s?.Trim();
                return v.ToJsonString().Trim('"').Trim();
            }
            return node.ToJsonString();
        }

        // names the reference can come under: PayLink's own (sic) "refernceId", or the provider's inside
        // "data" / "originalResponse" (Pine Labs: PlutusTransactionReferenceID)
        private static readonly string[] ReferenceNames =
            { "refernceId", "referenceId", "refId", "PlutusTransactionReferenceID", "transactionReferenceId" };

        /// <summary>
        /// The provider reference of a pushed transaction (needed by transstatus / canceltrans), or null.
        /// Looks at the top level first, then "data", then the provider's own JSON in "originalResponse".
        /// </summary>
        public static string? FindReference(JsonObject? body)
        {
            if (body == null) return null;

            var direct = FirstValue(body);
            if (direct != null) return direct;

            if (FindProperty(body, "data") is JsonObject data && FirstValue(data) is { } fromData)
                return fromData;

            var original = ReadString(body, "originalResponse");
            if (!string.IsNullOrWhiteSpace(original))
            {
                try
                {
                    if (JsonNode.Parse(original) is JsonObject inner)
                        return FirstValue(inner) ?? (FindProperty(inner, "data") is JsonObject d ? FirstValue(d) : null);
                }
                catch (JsonException) { }
            }
            return null;

            static string? FirstValue(JsonObject o)
            {
                foreach (var n in ReferenceNames)
                {
                    var v = ReadString(o, n);
                    if (!string.IsNullOrWhiteSpace(v) && v != "0" && !v.Equals("null", StringComparison.OrdinalIgnoreCase))
                        return v;
                }
                return null;
            }
        }

        public static int? ReadInt(JsonObject obj, string name)
        {
            var s = ReadString(obj, name);
            return int.TryParse(s, out var i) ? i : null;
        }

        public static decimal? ReadDecimal(JsonObject obj, string name)
        {
            var s = ReadString(obj, name);
            return decimal.TryParse(s, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : null;
        }

        private static JsonNode? FindProperty(JsonObject obj, string name)
        {
            foreach (var kv in obj)
                if (string.Equals(kv.Key, name, StringComparison.OrdinalIgnoreCase))
                    return kv.Value;
            return null;
        }
    }

    /// <summary>Masks secrets before anything is logged.</summary>
    public static class PayLinkMask
    {
        private static readonly HashSet<string> CardFields = new(StringComparer.OrdinalIgnoreCase)
            { "cardNo", "cardNumber", "pan" };

        private static readonly System.Text.RegularExpressions.Regex FullPan = new(@"\d{12,19}");

        /// <summary>
        /// For the DB log: the whole request / response as it was, no length cut.
        /// Hidden: password / token values, and a card number only when it is a full (unmasked) number.
        /// </summary>
        public static string Full(string json)
        {
            try
            {
                var node = JsonNode.Parse(json);
                if (node == null) return json;
                WalkFull(node);
                return node.ToJsonString(new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            }
            catch (JsonException)
            {
                return json;   // not JSON (HTML error page etc.): stored as it came
            }
        }

        private static void WalkFull(JsonNode node)
        {
            if (node is JsonObject obj)
            {
                foreach (var key in obj.Select(k => k.Key).ToList())
                {
                    var child = obj[key];
                    if (Hidden.Contains(key))
                        obj[key] = child == null ? null : "***";
                    else if (CardFields.Contains(key) && child is JsonValue)
                    {
                        var v = child.ToJsonString().Trim('"');
                        if (FullPan.IsMatch(v.Replace(" ", "").Replace("-", "")))
                            obj[key] = Last4(v);
                    }
                    else if (child != null)
                        WalkFull(child);
                }
            }
            else if (node is JsonArray arr)
            {
                foreach (var item in arr)
                    if (item != null) WalkFull(item);
            }
        }

        private static readonly HashSet<string> Hidden = new(StringComparer.OrdinalIgnoreCase)
            { "password", "token", "access_token", "authorization", "secret" };

        // keep only the last 4 characters
        private static readonly HashSet<string> Partial = new(StringComparer.OrdinalIgnoreCase)
            { "edcsNo", "cardNo", "cardNumber", "pan", "securityToken", "security_token" };

        public static string Json(string json)
        {
            try
            {
                var node = JsonNode.Parse(json);
                if (node == null) return json;
                Walk(node);
                return node.ToJsonString();
            }
            catch (JsonException)
            {
                // not JSON (HTML error page etc.): keep it short
                return json.Length > 2000 ? json[..2000] : json;
            }
        }

        private static void Walk(JsonNode node)
        {
            if (node is JsonObject obj)
            {
                foreach (var key in obj.Select(k => k.Key).ToList())
                {
                    var child = obj[key];
                    if (Hidden.Contains(key))
                        obj[key] = child == null ? null : "***";
                    else if (Partial.Contains(key) && child is JsonValue)
                        obj[key] = Last4(child.ToJsonString().Trim('"'));
                    else if (child != null)
                        Walk(child);
                }
            }
            else if (node is JsonArray arr)
            {
                foreach (var item in arr)
                    if (item != null) Walk(item);
            }
        }

        public static string Last4(string? value)
        {
            var v = (value ?? "").Trim();
            if (v.Length <= 4) return new string('X', v.Length);
            return new string('X', v.Length - 4) + v[^4..];
        }

        /// <summary>Card number from PayLink (may already be masked) -> last 4 digits, or null.</summary>
        public static string? CardLast4(string? cardNo)
        {
            var digits = new string((cardNo ?? "").Where(char.IsDigit).ToArray());
            var raw = (cardNo ?? "").Trim();
            if (raw.Length >= 4 && raw[^4..].All(char.IsDigit)) return raw[^4..];
            return digits.Length >= 4 ? digits[^4..] : null;
        }
    }
}
