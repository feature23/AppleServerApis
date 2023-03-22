using AppleServerApis.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using JWT;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Configuration;

namespace AppleServerApis.Tests;

public class AppleJwtTests
{
    private readonly string _base64PemPrivateKey;

    public AppleJwtTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AppleJwtTests>()
            .Build();
        _base64PemPrivateKey = configuration["Base64PemPrivateKey"]
            ?? throw new InvalidOperationException("Failed to load \"Base64PemPrivateKey\" from user secrets.");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("NOT_A_REAL_SUBJECT")]
    public void Create_NoSubject(string? subject)
    {
        // Arrange
        var options = new AppleJwtOptions { KeyId = "NOT_A_REAL_KEY_ID", TeamId = "NOT_A_REAL_TEAM_ID" };
        var algorithm = new AppleJwtPemKeyAlgorithm(_base64PemPrivateKey);
        
        // Act
        var jwt = AppleJwt.Create(options, algorithm, subject);

        // Assert
        using (new AssertionScope())
        {
            jwt.Should().NotBeNullOrEmpty();
            
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            var decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

            var header = decoder.DecodeHeader<JwtHeader>(jwt);
            var payload = decoder.DecodeToObject<IDictionary<string, object>>(new JwtParts(jwt), verify: false);

            header.Should().NotBeNull();
            payload.Should().NotBeNull();

            header.Algorithm.Should().Be("ES256");
            header.Type.Should().Be("JWT");
            header.KeyId.Should().Be("NOT_A_REAL_KEY_ID");
            
            payload.Keys.Count.Should().Be(subject is not null ? 5 : 4);
            payload.Keys.Should().Contain("exp");
            payload.Keys.Should().Contain("iat");
            payload.Keys.Should().Contain("nbf");
            payload.Keys.Should().Contain("iss");
            payload["iss"].Should().Be("NOT_A_REAL_TEAM_ID");

            if (subject is not null)
            {
                var headerDict = decoder.DecodeHeader<IDictionary<string, object>>(jwt);
                headerDict.Keys.Should().Contain("id");
                headerDict["id"].Should().Be($"NOT_A_REAL_TEAM_ID.{subject}");
                
                payload.Keys.Should().Contain("sub");
                payload["sub"].Should().Be(subject);
            }
        }
    }
}