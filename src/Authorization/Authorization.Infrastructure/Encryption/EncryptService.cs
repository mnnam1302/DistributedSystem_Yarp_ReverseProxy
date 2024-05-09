using Authorization.Application.Abstractions;
using System.Security.Cryptography;

namespace Authorization.Infrastructure.Encryption;

public class EncryptService : IEncryptService
{
    public (RSAParameters privateKey, RSAParameters publicKey) GenerateRsaKeyPair()
    {
        using (var rsa = RSA.Create(2048))
        {
            var privateKey = rsa.ExportParameters(true); // Xuất khóa riêng
            var publicKey = rsa.ExportParameters(false); // Xuất khóa công khai

            return (privateKey, publicKey);
        }
    }
}