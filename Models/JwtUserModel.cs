namespace Suvarna.DGMS.Api.Models
{
    public class JwtUserModel
    {
        public long USER_ID { get; set; }

        public string USER_NAME { get; set; } = string.Empty;

        public string ROLE_NAME { get; set; } = string.Empty;
    }
}
