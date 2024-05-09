using DistributedSystem.Contract.Abstractions.Message;

namespace DistributedSystem.Contract.Services.V1.Identity;

public static class Query
{
    public record GetLoginQuery(string Email, string Password) : IQuery<Response.Authenticated>;

    public record TokenQuery(string? AccessToken, string? RefreshToken) : IQuery<Response.Authenticated>;
}
