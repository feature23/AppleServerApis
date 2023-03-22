using JWT.Algorithms;
using JWT.Builder;

namespace AppleServerApis.Core.Internal;

internal static class AppleJwt
{
    public static Func<Task<string>> AuthorizationHeaderValueGetter(AppleJwtOptions options, IJwtAlgorithm algorithm,
        string? subject = null)
    {
        return () =>
        {
            var jwt = Create(options, algorithm, subject);
            return Task.FromResult(jwt);
        };
    }

    internal static string Create(AppleJwtOptions options, IJwtAlgorithm algorithm, string? subject = null)
    {
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

    private static JwtBuilder AddClaimIf(this JwtBuilder builder, bool condition, ClaimName claimName,
        string? claimValue)
    {
        return condition ? builder.AddClaim(claimName, claimValue) : builder;
    }
}
