using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace PaperFeed.Infrastructure.Models.Unsplash.Api;

[UsedImplicitly]
public class Urls
{
    [JsonPropertyName("raw")]
    public required string Raw { get; set; }

    [JsonPropertyName("full")]
    public required string Full { get; set; }

    [JsonPropertyName("regular")]
    public required string Regular { get; set; }

    [JsonPropertyName("small")]
    public required string Small { get; set; }

    [JsonPropertyName("thumb")]
    public required string Thumb { get; set; }
}