namespace Authorization.Application.Abstractions;

public interface IRSAKeyGenerator
{
    (string privateKey, string publicKey) GenerateRsaKeyPair();
}
