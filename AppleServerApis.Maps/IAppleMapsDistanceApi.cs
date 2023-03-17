using AppleServerApis.Maps.Extensions;
using Refit;

namespace AppleServerApis.Maps;

[Headers("Authorization: Bearer")]
public interface IAppleMapsDistanceApi
{
    [Get("/v1/etas")]
    Task<EtasResponse> GetEtas(string origin, string destinations);

    public async IAsyncEnumerable<EtaResult> GetEtas(Location origin, IEnumerable<Location> destinations)
    {
        var tasks = destinations.Chunk(10).Select(batch =>
        {
            var originParam = $"{origin.Latitude},{origin.Longitude}";
            var destinationsParam = string.Join("|", batch.Select(i => $"{i.Latitude},{i.Longitude}"));

            return GetEtas(originParam, destinationsParam);
        });

        foreach (var resultSet in await Task.WhenAll(tasks))
        {
            foreach (var eta in resultSet.Etas)
            {
                yield return eta;
            }
        }
    }
}
