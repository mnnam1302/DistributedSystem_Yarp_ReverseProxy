namespace Authorization.Application.Abstractions;

public interface IPasswordHasherService
{
    string HashPassword(string password, string salt);

    bool VerifyPassword(string plaintextPassword, string ciphertextPassword, string salt);

    string GenerateSalt();
}