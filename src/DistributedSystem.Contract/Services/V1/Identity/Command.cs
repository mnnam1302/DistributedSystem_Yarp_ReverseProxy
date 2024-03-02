using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;

namespace DistributedSystem.Contract.Services.V1.Identity
{
    public static class Command
    {
        public record RevokeTokenCommand(string AccessToken) : ICommand;
    }
}