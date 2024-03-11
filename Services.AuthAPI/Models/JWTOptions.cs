namespace Services.AuthAPI.Models
{
    public class JWTOptions
    {
        public string APISecrete { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
