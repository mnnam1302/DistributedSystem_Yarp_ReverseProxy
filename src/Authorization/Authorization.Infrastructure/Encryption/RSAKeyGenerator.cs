using Authorization.Application.Abstractions;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System.Security.Cryptography;

namespace Authorization.Infrastructure.Encryption;

public class RSAKeyGenerator : IRSAKeyGenerator
{
    /// <summary>
    /// Usage of this method var (publicKey, privateKey) = GenerateRsaKeyPair()
    /// </summary>
    /// <returns></returns>
    public (string privateKey, string publicKey) GenerateRsaKeyPair()
    {
        using (var rsa = new RSACryptoServiceProvider(4096))
        {
            var publicKey = ExportPublicKey(rsa);
            var privateKey = ExportPrivateKey(rsa);
            return (privateKey, publicKey);
        }
    }

    private static string ExportPublicKey(RSACryptoServiceProvider csp)
    {
        var parameters = csp.ExportParameters(false);
        using (var writer = new StringWriter())
        {
            var pemWriter = new PemWriter(writer);
            pemWriter.WriteObject(new RsaKeyParameters(
                                    false,
                                    new Org.BouncyCastle.Math.BigInteger(1, parameters.Modulus),
                                    new Org.BouncyCastle.Math.BigInteger(1, parameters.Exponent)));
            return writer.ToString();
        }
    }

    private static string ExportPrivateKey(RSACryptoServiceProvider csp)
    {
        var parameters = csp.ExportParameters(true);
        using (var writer = new StringWriter())
        {
            var pemWriter = new PemWriter(writer);
            pemWriter.WriteObject(new RsaPrivateCrtKeyParameters(
              new Org.BouncyCastle.Math.BigInteger(1, parameters.Modulus),
              new Org.BouncyCastle.Math.BigInteger(1, parameters.Exponent),
              new Org.BouncyCastle.Math.BigInteger(1, parameters.D),
              new Org.BouncyCastle.Math.BigInteger(1, parameters.P),
              new Org.BouncyCastle.Math.BigInteger(1, parameters.Q),
              new Org.BouncyCastle.Math.BigInteger(1, parameters.DP),
              new Org.BouncyCastle.Math.BigInteger(1, parameters.DQ),
              new Org.BouncyCastle.Math.BigInteger(1, parameters.InverseQ)
            ));
            return writer.ToString();
        }
    }
}
