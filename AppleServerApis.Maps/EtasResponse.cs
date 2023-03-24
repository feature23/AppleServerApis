namespace AppleServerApis.Maps;

/// <summary>
/// Represents a response from the Apple Maps Distance API.
/// </summary>
/// <param name="Etas">The estimated travel times.</param>
public record EtasResponse(IReadOnlyList<EtaResult> Etas);
