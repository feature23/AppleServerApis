using System.Security.Cryptography;
using JWT.Algorithms;
using JWT.Builder;

namespace AppleServerApis.Core;

internal static class AppleJwt
{
    public static Func<Task<string>> AuthorizationHeaderValueGetter(AppleJwtOptions options, IJwtAlgorithm algorithm, string? subject = null)
    {
        return () =>
        {
            var jwt = Create(options, algorithm, subject);
            return Task.FromResult(jwt);
        };
    }
    
    public static string Create(AppleJwtOptions options, IJwtAlgorithm algorithm, string? subject = null)
    {
        // var algorithm = (options.PrivateKeyBase64, options.KeyVaultKeyIdentifier) switch
        // {
        //     ({ } base64Key, _) => (IJwtAlgorithm) new ES256Algorithm(ECDsa.Create(), LoadBase64Key(base64Key)),
        //     (null, { } keyVaultKeyIdentifier) => new AppleJwtKeyVaultEs256Algorithm(keyVaultKeyIdentifier),
        //     _ => throw new InvalidOperationException("Azure KeyVault key identifier must be provided if base64-encoded private key is not provided.")
        // };
        
        var token = JwtBuilder.Create()
            .WithAlgorithm(algorithm)
            .AddHeader(HeaderName.Type, "JWT")
            .AddHeader(HeaderName.KeyId, options.KeyId)
            .AddHeaderIf(subject != null, "id", $"{options.TeamId}.{subject}")
            .AddClaim(ClaimName.ExpirationTime, DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds())
            .AddClaim(ClaimName.IssuedAt, DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            .AddClaim(ClaimName.NotBefore, DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeSeconds())
            .AddClaim(ClaimName.Issuer, options.TeamId)
            .AddClaimIf(subject != null, ClaimName.Subject, subject)
            .Encode();

        return token;
    }

    private static JwtBuilder AddHeaderIf(this JwtBuilder builder, bool condition, string header, string? value)
    {
        return condition ? builder.AddHeader(header, value) : builder;
    }
    
    private static JwtBuilder AddClaimIf(this JwtBuilder builder, bool condition, ClaimName claimName, string? claimValue)
    {
        return condition ? builder.AddClaim(claimName, claimValue) : builder;
    }

    private static ECDsa LoadBase64Key(string base64Key)
    {
        var derContents = ExtractDerFromPem(base64Key);
        var cngKey = CngKey.Import(derContents, CngKeyBlobFormat.EccPublicBlob);
        
        return new ECDsaCng(cngKey);

        // var key = ECDsa.Create();
        // key.ImportFromPem(base64Key);
        // return key;
    }
    
    // Helper function to extract DER data from a PEM string
    private static byte[] ExtractDerFromPem(string pem)
    {
        const string header = "-----BEGIN ";
        const string footer = "-----END ";
        var startIndex = pem.IndexOf(header, StringComparison.Ordinal) + header.Length;
        var endIndex = pem.IndexOf(footer, startIndex, StringComparison.Ordinal);
        var base64 = pem.Substring(startIndex, endIndex - startIndex);
        return Convert.FromBase64String(base64);
    }
}