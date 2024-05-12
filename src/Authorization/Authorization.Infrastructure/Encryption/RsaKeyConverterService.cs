using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace Authorization.Infrastructure.Encryption;

public static class RsaKeyConverterService
{
    /*
     * The RSA keys as plain strings (PEM format) instead of XML, you can convert the keys to PEM format using the PemUtils class from the Org.BouncyCastle.OpenSsl namespace
     */

    public static RSA CreateRsaFromPublicKey(string publicKey)
    {
        // Convert the PEM public key to PKCS#1 format
        publicKey = ConvertPemPublicKeyToPkcs1(publicKey);
        var publicKeyBytes = Convert.FromBase64String(publicKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKeyBytes, out _);
        return rsa;
    }

    public static RSA CreateRsaFromPrivateKey(string privateKey)
    {
        // Convert the PEM private key to PKCS#1 format
        privateKey = ConvertPemPrivateKeyToPkcs1(privateKey);
        var privateKeyBytes = Convert.FromBase64String(privateKey);
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        return rsa;
    }

    private static string ConvertPemPublicKeyToPkcs1(string publicKey)
    {
        var rsaKeyParameters = (RsaKeyParameters)new PemReader(new StringReader(publicKey)).ReadObject();
        var rsaParameters = new RSAParameters
        {
            Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
            Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned(),
        };
        using (var rsa = RSA.Create())
        {
            rsa.ImportParameters(rsaParameters);
            return Convert.ToBase64String(rsa.ExportRSAPublicKey());
        }
    }

    private static string ConvertPemPrivateKeyToPkcs1(string privateKey)
    {
        var rsaKeyPair = (AsymmetricCipherKeyPair)new PemReader(new StringReader(privateKey)).ReadObject();
        var rsaParams = ToRSAParameters((RsaPrivateCrtKeyParameters)rsaKeyPair.Private);
        using (var rsa = RSA.Create())
        {
            rsa.ImportParameters(rsaParams);
            return Convert.ToBase64String(rsa.ExportRSAPrivateKey());
        }
    }

    private static RSAParameters ToRSAParameters(RsaPrivateCrtKeyParameters privKey)
    {
        var rsaParams = new RSAParameters
        {
            Modulus = privKey.Modulus.ToByteArrayUnsigned(),
            Exponent = privKey.PublicExponent.ToByteArrayUnsigned(),
            D = privKey.Exponent.ToByteArrayUnsigned(),
            P = privKey.P.ToByteArrayUnsigned(),
            Q = privKey.Q.ToByteArrayUnsigned(),
            DP = privKey.DP.ToByteArrayUnsigned(),
            DQ = privKey.DQ.ToByteArrayUnsigned(),
            InverseQ = privKey.QInv.ToByteArrayUnsigned(),
        };
        return rsaParams;
    }
}
