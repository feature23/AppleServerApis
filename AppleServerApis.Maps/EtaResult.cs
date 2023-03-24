namespace AppleServerApis.Maps;

/// <summary>
/// Represents a single estimated travel time.
/// </summary>
/// <param name="Destination">The destination location.</param>
/// <param name="TransportType">The transport type.</param>
/// <param name="DistanceMeters">The distance in meters.</param>
/// <param name="ExpectedTravelTimeSeconds">The expected travel time in seconds.</param>
/// <param name="StaticTravelTimeSeconds">The static travel time in seconds.</param>
public record EtaResult(
    Location Destination,
    string TransportType,
    int DistanceMeters,
    int ExpectedTravelTimeSeconds,
    int StaticTravelTimeSeconds
);

/*
    {
      “destination”: {
        “latitude”: 37.32556561130194,
        “longitude”: -121.94635203581443
      },
      “transportType”: “AUTOMOBILE”,
      “distanceMeters”: 9550,
      “expectedTravelTimeSeconds”: 975,
      “staticTravelTimeSeconds”: 540
    }
*/
