using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;

namespace DistributedSystem.Contract.Services.V1.Identity
{
    public static class Command
    {
        public record RegisterUserCommand(string FirstName, string LastName, DateTime? DateOfBirth, string PhoneNumber, string Email, string Password, string PasswordConfirm) : ICommand;

        public record RevokeTokenCommand(string AccessToken) : ICommand;
    }
}