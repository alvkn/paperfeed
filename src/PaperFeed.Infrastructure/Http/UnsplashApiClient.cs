using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using PaperFeed.Infrastructure.Http.Abstractions;
using PaperFeed.Infrastructure.Models.Unsplash.Api.Endpoints;

namespace PaperFeed.Infrastructure.Http;

public class UnsplashApiClient : BaseApiClient, IUnsplashApiClient
{
    private readonly HttpClient _httpClient;

    public UnsplashApiClient(HttpClient httpClient, JsonSerializerOptions options) : base(options)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<GetRandomPhoto.Response>> GetRandomPhotos(
        GetRandomPhoto.Request request,
        CancellationToken cancellationToken)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["collections"] = string.Join(",", request.Collections ?? []),
            ["topics"] = string.Join(",", request.Topics ?? []),
            ["orientation"] = request.Orientation.ToString().ToLower(),
            ["username"] = request.Username,
            ["query"] = request.Query,
            ["content_filter"] = request.ContentFilter.ToString().ToLower(),
            ["count"] = request.Count.ToString()
        };

        var url = QueryHelpers.AddQueryString("/photos/random", queryParams);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await DeserializeResponse<IEnumerable<GetRandomPhoto.Response>>(response);
    }

    public async Task MarkAsDownloaded(
        string imageId,
        CancellationToken cancellationToken)
    {
        var escapedImageId = Uri.EscapeDataString(imageId);

        var response = await _httpClient.GetAsync($"/photos/{escapedImageId}/download", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}