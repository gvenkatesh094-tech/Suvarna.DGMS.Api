using System.Text.Json;
using System.Text.Json.Serialization;

namespace Suvarna.DGMS.Api.Helpers
{
    /// <summary>
    /// DB columns hold UTC (GETUTCDATE) but SQL "datetime" has no zone, so ADO.NET returns Kind=Unspecified.
    /// Write every DateTime as UTC ISO with 'Z' so the browser converts it to the user's local time.
    /// Incoming values with a zone are converted to UTC; zone-less values are taken as UTC.
    /// </summary>
    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
        {
            var d = r.GetDateTime();
            return d.Kind == DateTimeKind.Local ? d.ToUniversalTime() : DateTime.SpecifyKind(d, DateTimeKind.Utc);
        }
        public override void Write(Utf8JsonWriter w, DateTime v, JsonSerializerOptions o)
        {
            var u = v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : DateTime.SpecifyKind(v, DateTimeKind.Utc);
            w.WriteStringValue(u.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"));
        }
    }

    public class UtcNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private static readonly UtcDateTimeConverter Inner = new();
        public override DateTime? Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
            => r.TokenType == JsonTokenType.Null ? null : Inner.Read(ref r, typeof(DateTime), o);
        public override void Write(Utf8JsonWriter w, DateTime? v, JsonSerializerOptions o)
        { if (v.HasValue) Inner.Write(w, v.Value, o); else w.WriteNullValue(); }
    }
}
