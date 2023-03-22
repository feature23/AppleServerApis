using System.Security.Cryptography;
using AppleServerApis.Core;
using AppleServerApis.Core.Internal;
using JWT.Algorithms;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AppleServerApis.Maps;

public static class Dependencies
{
    public static IAppleServerApiClientBuilder AddAppleMapsApiServices(this IServiceCollection services,
        AppleJwtOptions jwtOptions,
        Action<Type, IHttpClientBuilder>? configureHttpClientBuilder = null)
    {
        services.AddAppleCoreApiServices();
        
        const string hostUrl = "https://maps-api.apple.com";
        var baseAddress = new Uri(hostUrl);

        var apiClientBuilder = new AppleServerApiClientBuilder();
        
        var apiInterfaceTypes = new[] { typeof(IAppleMapsDistanceApi) };
        
        foreach (var interfaceType in apiInterfaceTypes)
        {
            var algorithm = new DelegatedJwtAlgorithm(() => apiClientBuilder.JwtAlgorithm);
            
            var httpClientBuilder = services.AddRefitClient(
                    interfaceType, 
                    sp => CreateRefitSettings(sp, hostUrl, jwtOptions, algorithm)
                )
                .ConfigureHttpClient(http => http.BaseAddress = baseAddress);

            configureHttpClientBuilder?.Invoke(interfaceType, httpClientBuilder);
        }

        return apiClientBuilder;
    }

    internal static RefitSettings CreateRefitSettings(IServiceProvider sp, string hostUrl, AppleJwtOptions jwtOptions, IAsymmetricAlgorithm algorithm) => new()
    {
        AuthorizationHeaderValueGetter = AppleJwtAuthenticator.Create(sp, hostUrl, jwtOptions, algorithm)
    };

    private class DelegatedJwtAlgorithm : IAsymmetricAlgorithm
    {
        private readonly Lazy<IAsymmetricAlgorithm> _lazyAlgorithm;

        public DelegatedJwtAlgorithm(Func<IAsymmetricAlgorithm> algorithmProvider)
        {
            _lazyAlgorithm = new Lazy<IAsymmetricAlgorithm>(algorithmProvider);
        }

        public string Name => _lazyAlgorithm.Value.Name;

        public HashAlgorithmName HashAlgorithmName => _lazyAlgorithm.Value.HashAlgorithmName;
        
        public byte[] Sign(byte[] key, byte[] bytesToSign) =>
            _lazyAlgorithm.Value.Sign(key, bytesToSign);

        public bool Verify(byte[] bytesToSign, byte[] signature) =>
            _lazyAlgorithm.Value.Verify(bytesToSign, signature);
    }
}
