namespace AppleServerApis.Maps;

public record Location(double Latitude, double Longitude)
{
    private const double EarthRadiusInKilometers = 6367;
    
    // Inspired by implementation here: https://github.com/bamcis-io/GeoCoordinate/blob/main/GeoCoordinate/GeoCoordinate.cs
    public double DistanceTo(Location destination)
    {
        var radius = EarthRadiusInKilometers * 1000;

        var deltaLatitude = DegreeToRadian(destination.Latitude - Latitude);
        var deltaLongitude = DegreeToRadian(destination.Longitude - Longitude);
        var a = Math.Pow(Math.Sin(deltaLatitude / 2), 2) +
                Math.Cos(DegreeToRadian(Latitude)) * Math.Cos(DegreeToRadian(destination.Latitude)) *
                Math.Pow(Math.Sin(deltaLongitude / 2), 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = c * radius;

        return distance;
    }
    
    private static double DegreeToRadian(double degree)
    {
        return degree * (Math.PI / 180);
    }
}
