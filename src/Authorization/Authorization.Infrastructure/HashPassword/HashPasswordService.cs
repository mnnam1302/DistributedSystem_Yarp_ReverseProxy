using Authorization.Application.Abstractions;
using System.Security.Cryptography;

namespace Authorization.Infrastructure.HashPassword
{
    public sealed class HashPasswordService : IHashPasswordService
    {
        private readonly int keySize = 64;
        private readonly int iteration = 30000;
        private readonly HashAlgorithmName passwordHashAlgorithm = HashAlgorithmName.SHA512;

        public string HashPassword(string password, out string salt)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var saltByte = new byte[keySize];
                rng.GetBytes(saltByte);

                var hashPassword = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    saltByte,
                    iteration,
                    passwordHashAlgorithm,
                    keySize);

                // Convert byte[] to string to store in DB
                salt = Convert.ToBase64String(saltByte);
                return Convert.ToBase64String(saltByte.Concat(hashPassword).ToArray());
            }
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            // Convert string to byte[] to Encrypt and Compare
            var saltByte = Convert.FromBase64String(salt);

            var newHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    saltByte,
                    iteration,
                    passwordHashAlgorithm,
                    keySize);

            var newHashPassword = Convert.ToBase64String(saltByte.Concat(newHash).ToArray());
            return newHashPassword.Equals(hash);
        }
    }
}