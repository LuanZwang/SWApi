using System.Text.Json.Serialization;

namespace SWApi.Domain.Dto.Api.Planet
{
    public class FilmDto
    {
        [JsonPropertyName("title")]
        public string Title { get; init; }

        [JsonPropertyName("director")]
        public string Director { get; init; }

        [JsonPropertyName("releaseDate")]
        public string ReleaseDate { get; init; }
    }
}