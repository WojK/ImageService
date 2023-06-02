namespace ImagesService
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public string Issuer { get; set; }
        public double JwtExpiresDays { get; set; }

        public static AuthenticationSettings authSettings = new AuthenticationSettings();
    }
}

