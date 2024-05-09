namespace Authorization.Application.Abstractions;

public interface IPasswordHasherService
{
    // Version 01
    string HashPassword(string password, string salt);

    bool VerifyPassword(string plaintextPassword, string ciphertextPassword, string salt);

    string GenerateSalt();

    // Version 02
}
