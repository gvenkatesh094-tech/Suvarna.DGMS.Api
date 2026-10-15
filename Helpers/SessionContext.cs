using Microsoft.AspNetCore.Http;

namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// The DB session id of the current request (header "X-Session-Id", sent by the Angular interceptor).
    /// Repositories pass it to stored procedures (@IP_SESSION_ID); the SP resolves group / org / location / user
    /// from it, so a screen can never choose its own tenant. SessionBindingFilter checks it belongs to the JWT user.
    /// </summary>
    public class SessionContext
    {
        private readonly IHttpContextAccessor _http;
        public SessionContext(IHttpContextAccessor http) { _http = http; }

        public string? SessionId
        {
            get
            {
                var v = _http.HttpContext?.Request.Headers["X-Session-Id"].ToString();
                return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
            }
        }

        /// <summary>Header first; the request body's sessionId only as a fallback for older clients.</summary>
        public string Require(int bodySessionId = 0)
        {
            var id = SessionId ?? (bodySessionId > 0 ? bodySessionId.ToString() : null);
            if (id is null) throw new UnauthorizedAccessException("Session id is missing. Please login again.");
            return id;
        }
    }
}
