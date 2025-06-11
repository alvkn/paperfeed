using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace PaperFeed.Infrastructure.Models.Unsplash.Api.Endpoints;

public abstract class GetRandomPhoto
{
    public record Request(
        IEnumerable<string>? Collections = null!,
        IEnumerable<string>? Topics = null!,
        Orientation Orientation = Orientation.Landscape,
        string Username = "",
        string Query = "",
        ContentFilter ContentFilter = ContentFilter.Low,
        int Count = 1);

    [UsedImplicitly]
    public class Response
    {
        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("urls")]
        public required Urls Urls { get; init; }
    }
}