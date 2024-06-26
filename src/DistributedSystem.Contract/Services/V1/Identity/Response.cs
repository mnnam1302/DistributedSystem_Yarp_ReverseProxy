﻿namespace DistributedSystem.Contract.Services.V1.Identity;

public static class Response
{
    public record Authenticated
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public DateTime? RefreshTokenExpiryTime { get; init; }
    }
}
