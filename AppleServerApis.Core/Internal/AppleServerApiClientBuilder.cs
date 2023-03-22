using System.Diagnostics.CodeAnalysis;
using JWT.Algorithms;

namespace AppleServerApis.Core.Internal;

public class AppleServerApiClientBuilder : IAppleServerApiClientBuilder
{
    private IAsymmetricAlgorithm? _algorithm;
    
    public IAsymmetricAlgorithm JwtAlgorithm
    {
        get
        {
            if (_algorithm is null)
            {
                ThrowHelper.ThrowAlgorithmHasNotBeenSet();
            }
            return _algorithm;
        }
    }

    public IAppleServerApiClientBuilder WithJwtAlgorithm(IAsymmetricAlgorithm algorithm)
    {
        _algorithm = algorithm;
        return this;
    }

    private static class ThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowAlgorithmHasNotBeenSet()
        {
            const string message = $"JWT algorithm has not been set. Call {nameof(IAppleServerApiClientBuilder.WithJwtAlgorithm)} on {nameof(IAppleServerApiClientBuilder)}.";
            throw new InvalidOperationException(message);
        } 
    }
}
