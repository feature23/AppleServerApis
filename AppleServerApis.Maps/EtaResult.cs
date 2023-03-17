namespace AppleServerApis.Maps;

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
