using AppleServerApis.Core;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AppleServerApis.Maps;

public static class Dependencies
{
    public static IServiceCollection AddAppleMapsApiServices(this IServiceCollection services,
        AppleJwtOptions jwtOptions,
        Action<Type, IHttpClientBuilder>? configureHttpClientBuilder = null)
    {
        const string hostUrl = "https://maps-api.apple.com";
        var baseAddress = new Uri(hostUrl);

        var apiInterfaceTypes = new[] { typeof(IAppleMapsDistanceApi) };
        
        foreach (var interfaceType in apiInterfaceTypes)
        {
            var httpClientBuilder = services.AddRefitClient(interfaceType, sp => new RefitSettings
                {
                    AuthorizationHeaderValueGetter = AppleJwtAuthenticator.Create(sp, hostUrl, jwtOptions)
                })
                .ConfigureHttpClient(http => http.BaseAddress = baseAddress);

            configureHttpClientBuilder?.Invoke(interfaceType, httpClientBuilder);
        }
        
        return services;
    }
}
