using System.Security.Cryptography;

namespace Authorization.Application.Abstractions;

public interface IEncryptService
{
    (RSAParameters privateKey, RSAParameters publicKey) GenerateRsaKeyPair();
}