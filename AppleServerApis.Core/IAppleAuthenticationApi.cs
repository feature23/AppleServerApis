using Refit;

namespace AppleServerApis.Core;

[Headers("Authorization: Bearer")]
internal interface IAppleAuthenticationApi
{
    [Get("/v1/token")]
    Task<TokenResponse> GetToken();
}