using AngleSharp;
using PaperFeed.Infrastructure.Http.Abstractions;

namespace PaperFeed.Infrastructure.Http;

public class UnsplashHttpScrapper : IUnsplashHttpScrapper
{
    private readonly HttpClient _httpClient;
    private readonly IBrowsingContext _browsingContext;

    private const string TagsSelector = "[data-testid='photos-route'] a[href^='/s']";

    public UnsplashHttpScrapper(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _browsingContext = BrowsingContext.New(Configuration.Default);
    }

    public async Task<IEnumerable<string>> GetPhotoTags(
        string imageId,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetStreamAsync("/photos/" + imageId, cancellationToken);

        var document = await _browsingContext.OpenAsync(
            req => req.Content(response, shouldDispose: true),
            cancellationToken);

        return document.QuerySelectorAll(TagsSelector)
            .Select(x => x.TextContent)
            .ToArray();
    }
}