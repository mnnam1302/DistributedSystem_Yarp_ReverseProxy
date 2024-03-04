namespace ApiGateway.DependecyInjection.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public short ExpireMin { get; set; }
    }
}
