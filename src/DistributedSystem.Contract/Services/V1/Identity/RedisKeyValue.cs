namespace DistributedSystem.Contract.Services.V1.Identity;

public static class RedisKeyValue
{
    public record AuthenticatedValue
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public DateTime? RefreshTokenExpiryTime { get; init; }
        public string? PublicKey { get; init; }
    }
}
