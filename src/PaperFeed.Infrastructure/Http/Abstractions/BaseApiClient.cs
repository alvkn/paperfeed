using System.Text.Json;

namespace PaperFeed.Infrastructure.Http.Abstractions;

public abstract class BaseApiClient
{
    private readonly JsonSerializerOptions _options;

    protected BaseApiClient(JsonSerializerOptions options)
    {
        _options = options;
    }

    protected async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        return (await JsonSerializer.DeserializeAsync<T>(stream, _options))!;
    }
}