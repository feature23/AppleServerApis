using AppleServerApis.Maps;

namespace AppleServerApis.Tests;

[Trait(Traits.Category, Traits.Categories.Unit)]
public class LocationTests
{
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(0, 0, 0, 1, 111)]
    [InlineData(0, 0, 1, 0, 111)]
    [InlineData(0, 0, 1, 1, 157)]
    [Theory]
    public void DistanceToTests(double latitudeFrom, 
        double longitudeFrom, 
        double latitudeTo, 
        double longitudeTo,
        int distanceRoundedKm)
    {
        var from = new Location(latitudeFrom, longitudeFrom);
        var to = new Location(latitudeTo, longitudeTo);
        
        var actualDistance = (int)Math.Floor(from.DistanceTo(to) / 1000);
        
        Assert.Equal(distanceRoundedKm, actualDistance);
    }
}