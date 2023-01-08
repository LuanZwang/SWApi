using System.Text.Json.Serialization;

namespace SWApi.Domain.Dto.Api.Commom;

public class GetAllDto<T>
{
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("totalCount")]
    public long TotalCount { get; set; }

    [JsonPropertyName("nextPage")]
    public int? NextPage { get; set; }

    [JsonPropertyName("previousPage")]
    public int? PreviousPage { get; set; }


    [JsonPropertyName("items")]
    public IEnumerable<T> Items { get; set; }
}