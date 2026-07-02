namespace Suvarna.DGMS.Api.Models
{
    public class RefreshTokenEntity
    {
        public long USER_ID { get; set; }

        public string REFRESH_TOKEN { get; set; } = string.Empty;

        public DateTime EXPIRY_DATE { get; set; }

        public string IS_REVOKED { get; set; } = string.Empty;
    }
}
