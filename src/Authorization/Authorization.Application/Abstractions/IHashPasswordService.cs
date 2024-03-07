namespace Authorization.Application.Abstractions
{
    public interface IHashPasswordService
    {
        string HashPassword(string password, out string salt);

        bool VerifyPassword(string password, string hash, string salt);
    }
}