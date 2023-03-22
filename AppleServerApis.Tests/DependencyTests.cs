using System.Reflection;
using AppleServerApis.Core;
using AppleServerApis.Maps;
using AppleServerApis.Tests.Support;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace AppleServerApis.Tests;

public class DependencyTests
{
    private readonly string _base64PemPrivateKey;
    private readonly string _teamId;
    private readonly string _keyId;

    public DependencyTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AppleJwtTests>()
            .Build();
        _base64PemPrivateKey = configuration["Base64PemPrivateKey"]
            ?? throw new InvalidOperationException("Failed to load \"Base64PemPrivateKey\" from user secrets.");
        _teamId = configuration["TeamId"]
                  ?? throw new InvalidOperationException("Failed to load \"Base64PemPrivateKey\" from user secrets.");
        _keyId = configuration["KeyId"]
                 ?? throw new InvalidOperationException("Failed to load \"Base64PemPrivateKey\" from user secrets.");
    }

    [Fact]
    public async Task TestCoreDependencies()
    {
        // Arrange
        IServiceCollection services = new ServiceCollection();
        var options = new AppleJwtOptions
        {
            TeamId = _teamId,
            KeyId = _keyId
        };

        // Act
        services.AddAppleMapsApiServices(options)
            .WithJwtAlgorithm(new AppleJwtPemKeyAlgorithm(_base64PemPrivateKey));
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var apiClient = serviceProvider.GetService<IAppleMapsDistanceApi>();

        using (new AssertionScope())
        {
            apiClient.Should().NotBeNull();

            var httpClient = apiClient!.GetPrivateHttpClient();

            httpClient.BaseAddress.Should().NotBeNull();
            httpClient.BaseAddress!.AbsoluteUri.Should().Be("https://maps-api.apple.com/");

            var refitSettings = apiClient!.GetPrivateRefitSettings();

            refitSettings.AuthorizationHeaderValueGetter.Should().NotBeNull();

            var header = await refitSettings.AuthorizationHeaderValueGetter!();
            header.Should().NotBeNullOrEmpty();
        }
    }
}
