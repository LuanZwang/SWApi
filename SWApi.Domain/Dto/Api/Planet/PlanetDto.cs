using System.Text.Json.Serialization;

namespace SWApi.Domain.Dto.Api.Planet;

public class PlanetDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }
    [JsonPropertyName("climate")]
    public string Climate { get; init; }

    [JsonPropertyName("terrain")]
    public string Terrain { get; init; }

    [JsonPropertyName("films")]
    public IEnumerable<FilmDto> Films { get; init; }
}
