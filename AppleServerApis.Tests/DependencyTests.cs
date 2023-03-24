using AppleServerApis.Core;
using AppleServerApis.Maps;
using AppleServerApis.Tests.Support;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppleServerApis.Tests;

[Trait(Traits.Category, Traits.Categories.Integration)]
public class DependencyTests
{
    private readonly string _base64PemPrivateKey;
    private readonly string _teamId;
    private readonly string _keyId;

    public DependencyTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AppleJwtTests>()
            .AddEnvironmentVariables()
            .Build();
        _base64PemPrivateKey = configuration["Base64PemPrivateKey"] ?? configuration["BASE64_PEM_PRIVATE_KEY"]
            ?? throw new InvalidOperationException("Failed to load \"Base64PemPrivateKey\" or \"BASE64_PEM_PRIVATE_KEY\" from configuration.");
        _teamId = configuration["TeamId"] ?? configuration["TEAM_ID"]
                  ?? throw new InvalidOperationException("Failed to load \"TeamId\" or \"TEAM_ID\" from configuration.");
        _keyId = configuration["KeyId"] ?? configuration["KEY_ID"]
                 ?? throw new InvalidOperationException("Failed to load \"KeyId\" or \"KEY_ID\" from configuration.");
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
