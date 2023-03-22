using Refit;

namespace AppleServerApis.Core.Internal;

[Headers("Authorization: Bearer")]
internal interface IAppleAuthenticationApi
{
    [Get("/v1/token")]
    Task<TokenResponse> GetToken();
}
