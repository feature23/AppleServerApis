using System.Security.Cryptography;
using JWT.Algorithms;

namespace AppleServerApis.Core;

public class AppleJwtPemKeyAlgorithm : IAsymmetricAlgorithm
{
    private readonly ES256Algorithm _algorithm;

    public AppleJwtPemKeyAlgorithm(string base64PemPrivateKey)
    {
        _algorithm = new ES256Algorithm(ECDsa.Create(), LoadBase64Key(base64PemPrivateKey));
    }

    public byte[] Sign(byte[] key, byte[] bytesToSign) => _algorithm.Sign(key, bytesToSign);

    public string Name => _algorithm.Name;

    public HashAlgorithmName HashAlgorithmName => _algorithm.HashAlgorithmName;

    public bool Verify(byte[] bytesToSign, byte[] signature) => _algorithm.Verify(bytesToSign, signature);
    
    private static ECDsa LoadBase64Key(string base64PemPrivateKey)
    {
        var ecdsa = ECDsa.Create()
                    ?? throw new InvalidOperationException("Failed to create ECDsa key.");
        ecdsa.ImportFromPem(base64PemPrivateKey);
        return ecdsa;
    }
}