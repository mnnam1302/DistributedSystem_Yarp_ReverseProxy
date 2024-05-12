using DistributedSystem.Contract.Abstractions.Message;

namespace DistributedSystem.Contract.Services.V1.Identity;

public static class Command
{
    public record RegisterUserCommand(string FirstName, string LastName, DateTime? DateOfBirth, string PhoneNumber, string Email, string Password, string PasswordConfirm) : ICommand;

    public record RevokeTokenCommand(string Email, string AccessToken) : ICommand;

    // Phải suy nghĩ các trường hợp chỉ gửi Email thì sao?
    // Nếu một ai đó lấy email đại của ai đó gửi đi XÓA token ngta trên Redis
    // Nếu có access token thì đỡ hơn?
    public record LogoutCommand(string Email, string AccessToken) : ICommand;
}
