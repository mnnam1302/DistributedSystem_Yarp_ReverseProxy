using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;

namespace DistributedSystem.Contract.Services.V1.Identity
{
    public static class Command
    {
        public record RegisterCommand(string FirstName, string LastName, DateTime DateOfBirth, string PhoneNumber, string Email, string Password) : ICommand;
        public record RevokeTokenCommand(string AccessToken) : ICommand;
    }
}