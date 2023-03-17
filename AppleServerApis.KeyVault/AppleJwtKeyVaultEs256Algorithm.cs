using System.Security.Cryptography;
using JWT.Algorithms;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace AppleServerApis.KeyVault;

public class AppleJwtKeyVaultEs256Algorithm : IAsymmetricAlgorithm
{
    private readonly string _keyVaultKeyId;
    private readonly IKeyVaultClient _client;
    private readonly HashAlgorithm _hash = SHA256.Create();

    public AppleJwtKeyVaultEs256Algorithm(string keyVaultKeyId)
    {
        _keyVaultKeyId = keyVaultKeyId;
        var authCallback = new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback);
        _client = new KeyVaultClient(authCallback);
    }

    public byte[] Sign(byte[] key, byte[] bytesToSign)
    {
        return SignAsync(bytesToSign).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public string Name => nameof(JwtAlgorithmName.ES256);
    
    public HashAlgorithmName HashAlgorithmName => HashAlgorithmName.SHA256;

    public bool Verify(byte[] bytesToSign, byte[] signature) =>
        throw new NotSupportedException();

    private async Task<byte[]> SignAsync(byte[] bytesToSign)
    {
        if (bytesToSign == null || bytesToSign.Length == 0)
        {
            throw new ArgumentNullException(nameof(bytesToSign));
        }

        return (await _client.SignAsync(_keyVaultKeyId, "ES256", _hash.ComputeHash(bytesToSign))).Result;
    }
}
