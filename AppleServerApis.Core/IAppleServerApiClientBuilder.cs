using JWT.Algorithms;

namespace AppleServerApis.Core;

public interface IAppleServerApiClientBuilder
{
    IAsymmetricAlgorithm JwtAlgorithm { get; }
    
    IAppleServerApiClientBuilder WithJwtAlgorithm(IAsymmetricAlgorithm algorithm);

    public IAppleServerApiClientBuilder WithBase64PemJwtKey(string base64PemJwtKey) => 
        WithJwtAlgorithm(new AppleJwtPemKeyAlgorithm(base64PemJwtKey));
}
