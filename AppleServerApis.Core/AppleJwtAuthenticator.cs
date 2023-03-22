using JWT.Algorithms;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AppleServerApis.Core;

public static class AppleJwtAuthenticator
{
    public static Func<Task<string>> Create(IServiceProvider sp, string baseUrl, AppleJwtOptions jwtOptions, IJwtAlgorithm jwtAlgorithm)
    {
        var authenticationApi = RestService.For<IAppleAuthenticationApi>(baseUrl, new RefitSettings
        {
            AuthorizationHeaderValueGetter = AppleJwt.AuthorizationHeaderValueGetter(jwtOptions, jwtAlgorithm)
        });

        return async () =>
        {
            var cacheKey = $"AppleServerApiAuthentication_{baseUrl}";

            var cache = sp.GetRequiredService<IMemoryCache>();

            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                var (accessToken, expiresInSeconds) = await authenticationApi.GetToken();

                var cacheExpiration = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds - 60);

                entry.AbsoluteExpiration = cacheExpiration;

                return accessToken;
            });
        };
    }
}
