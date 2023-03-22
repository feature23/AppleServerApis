using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace AppleServerApis.Core;

public static class Dependencies
{
    private static readonly object LockObj = new();
    private static bool _hasRegisteredEncodingProvider;
    
    public static IServiceCollection AddAppleCoreApiServices(this IServiceCollection services)
    {
        // This is to deal with Apple's nonstandard "utf8" (instead of utf-8) Content-Type header encoding.
        if (!_hasRegisteredEncodingProvider)
        {
            lock (LockObj)
            {
                if (!_hasRegisteredEncodingProvider)
                {
                    Encoding.RegisterProvider(new AppleUtf8EncodingProvider());
                    _hasRegisteredEncodingProvider = true;
                }
            }
        }

        services.AddMemoryCache();

        return services;
    }
}
