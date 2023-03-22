using System.Reflection;
using AppleServerApis.Maps;
using Refit;

namespace AppleServerApis.Tests.Support;

internal static class RefitSettingsReflectionExtensions
{
    public static RefitSettings GetPrivateRefitSettings(this IAppleMapsDistanceApi apiClient)
    {
        const BindingFlags privateFieldFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        
        var type = apiClient!.GetType();
        
        var requestBuilderField = type.GetField("requestBuilder", privateFieldFlags)!;
        var requestBuilder = requestBuilderField.GetValue(apiClient)!;
            
        var innerBuilderField = requestBuilder.GetType().BaseType!.GetField("innerBuilder", privateFieldFlags)!;
        var innerBuilder = innerBuilderField.GetValue(requestBuilder)!;
            
        var settingsField = innerBuilder.GetType().BaseType!.GetField("settings", privateFieldFlags)!;
        var settings = (RefitSettings)settingsField.GetValue(innerBuilder)!;

        return settings;
    }

    public static HttpClient GetPrivateHttpClient(this IAppleMapsDistanceApi apiClient)
    {
        var type = apiClient!.GetType();
        
        var clientProperty = type.GetProperty("Client");
        var client = (HttpClient)clientProperty!.GetValue(apiClient)!;

        return client;
    }
}