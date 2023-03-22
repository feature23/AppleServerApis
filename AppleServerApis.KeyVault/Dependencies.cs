using AppleServerApis.Core;

namespace AppleServerApis.KeyVault;

public static class Dependencies
{
    /// <summary>
    /// Registers support for signing JWTs with an Azure KeyVault EC256 private key.
    /// </summary>
    /// <param name="builder">The <see cref="IAppleServerApiClientBuilder"/> instance</param>
    /// <param name="keyVaultKeyId">The ID of the Azure Key Vault private key to use</param>
    /// <returns>The modified <see cref="IAppleServerApiClientBuilder"/> instance</returns>
    public static IAppleServerApiClientBuilder WithKeyVaultKey(this IAppleServerApiClientBuilder builder, string keyVaultKeyId) =>
        builder.WithJwtAlgorithm(new AppleJwtKeyVaultEs256Algorithm(keyVaultKeyId));
}
