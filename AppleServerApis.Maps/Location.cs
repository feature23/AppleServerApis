namespace AppleServerApis.Maps;

/// <summary>
/// Constructs a location with the given latitude and longitude.
/// </summary>
/// <param name="Latitude">The latitude in degrees.</param>
/// <param name="Longitude">The longitude in degrees.</param>
public record Location(double Latitude, double Longitude)
{
    private const double EarthRadiusInKilometers = 6367;
    
    /// <summary>
    /// Computes the distance in meters between this location and the destination.
    /// </summary>
    /// <param name="destination">The destination location.</param>
    /// <returns>Returns the distance in meters.</returns>
    /// <remarks>Inspired by MIT-licensed implementation here: https://github.com/bamcis-io/GeoCoordinate/blob/main/GeoCoordinate/GeoCoordinate.cs</remarks>
    public double DistanceTo(Location destination)
    {
        const double radius = EarthRadiusInKilometers * 1000;

        var deltaLatitude = DegreeToRadian(destination.Latitude - Latitude);
        var deltaLongitude = DegreeToRadian(destination.Longitude - Longitude);
        var a = Math.Pow(Math.Sin(deltaLatitude / 2), 2) +
                Math.Cos(DegreeToRadian(Latitude)) * Math.Cos(DegreeToRadian(destination.Latitude)) *
                Math.Pow(Math.Sin(deltaLongitude / 2), 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = c * radius;

        return distance;
    }
    
    private static double DegreeToRadian(double degree) => degree * (Math.PI / 180);
}
