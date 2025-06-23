using Microsoft.Extensions.Logging;
using PaperFeed.Application.Abstractions.Repositories;
using PaperFeed.Application.Models;
using PaperFeed.Infrastructure.Http.Abstractions;
using PaperFeed.Infrastructure.Models.Unsplash.Api;
using PaperFeed.Infrastructure.Models.Unsplash.Api.Endpoints;

namespace PaperFeed.Infrastructure.StockRepositories;

public class UnsplashStockRepository : IImageStockRepository
{
    private readonly IUnsplashApiClient _apiClient;
    private readonly IUnsplashHttpScrapper _httpScrapper;
    private readonly ILogger<UnsplashStockRepository> _logger;

    private const string UnsplashPhotoPagePrefix = "https://unsplash.com/photos/";

    private readonly string[] _topics =
    [
        "wallpapers",
        "nature",
        "3d-renders",
        "textures-patterns",
        "travel",
        "architecture-interior"
    ];

    private readonly string[] _forbiddenTags =
    [
        "human",
        "face",
        "female",
        "male",
        "man",
        "men",
        "woman",
        "women",
        "people",
        "kid",
        "child",
        "baby",
        "children",
        "kids"
    ];

    public UnsplashStockRepository(
        IUnsplashApiClient apiClient,
        IUnsplashHttpScrapper httpScrapper,
        ILogger<UnsplashStockRepository> logger)
    {
        _apiClient = apiClient;
        _httpScrapper = httpScrapper;
        _logger = logger;
    }

    public async Task<StockImage?> GetNextImage(CancellationToken cancellationToken)
    {
        var request = new GetRandomPhoto.Request(
            Topics: _topics,
            Orientation: Orientation.Portrait,
            Count: 30);

        var photosResponse = (await _apiClient.GetRandomPhotos(request, cancellationToken)).ToArray();

        _logger.LogInformation("Got {count} random photos", photosResponse.Length);

        foreach (var photo in photosResponse)
        {
            if (!await HasForbiddenTags(photo.Id, cancellationToken))
            {
                _logger.LogInformation("Found photo without forbindden tags with id {photoId}", photo.Id);

                return new StockImage(photo.Id, photo.Urls.Raw, photo.Urls.Regular, UnsplashPhotoPagePrefix + photo.Id);
            }

            _logger.LogInformation("Photo with id {photoId} has forbidden tags", photo.Id);
        }

        _logger.LogWarning("No photos without forbidden tags was found");
        return null;
    }

    public async Task MarkAsDownloaded(string imageId, CancellationToken cancellationToken)
    {
        await _apiClient.MarkAsDownloaded(imageId, cancellationToken);
        _logger.LogInformation("Marked as downloaded image with id {imageId}", imageId);
    }

    private async Task<bool> HasForbiddenTags(string photoId, CancellationToken cancellationToken)
    {
        var scrappedTags = await _httpScrapper.GetPhotoTags(photoId, cancellationToken);

        return scrappedTags.Any(x => _forbiddenTags.Contains(x, StringComparer.OrdinalIgnoreCase));
    }
}