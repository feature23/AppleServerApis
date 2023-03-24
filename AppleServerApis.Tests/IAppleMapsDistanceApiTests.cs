using AppleServerApis.Maps;

namespace AppleServerApis.Tests;

public class IAppleMapsDistanceApiTests
{
    private class MockAppleMapsDistanceApi : IAppleMapsDistanceApi
    {
        public Task<EtasResponse> GetEtas(string origin, string destinations)
        {
            var splitDestinations = destinations.Split('|');
            
            var etas = splitDestinations.Select(d =>
            {
                var splitDestination = d.Split(',');
                var latitude = double.Parse(splitDestination[0]);
                var longitude = double.Parse(splitDestination[1]);
                return new EtaResult(
                    new Location(latitude, longitude),
                    "AUTOMOBILE",
                    9550,
                    975,
                    540
                );
            });
            
            var response = new EtasResponse(etas.ToList());
            return Task.FromResult(response);
        }
    }
    
    [Fact]
    public async Task LocationBasedGetEtasTest()
    {
        IAppleMapsDistanceApi api = new MockAppleMapsDistanceApi();
        var origin = new Location(0, 0);
        var destinations = new[]
        {
            new Location(0, 0),
            new Location(0, 1),
            new Location(1, 0),
            new Location(1, 1)
        };
        
        var etas = await api.GetEtas(origin, destinations).ToListAsync();
        
        Assert.Equal(4, etas.Count);
    }
}