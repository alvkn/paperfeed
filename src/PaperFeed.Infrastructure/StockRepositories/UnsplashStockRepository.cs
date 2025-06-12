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
    private readonly ILogger<UnsplashStockRepository> _logger;

    private const string UnsplashPhotoPagePrefix = "https://unsplash.com/photos/";

    private readonly string[] _topics =
    [
        "nature",
        "3d-renders",
        "textures-patterns",
        "travel",
        "architecture-interior"
    ];

    public UnsplashStockRepository(
        IUnsplashApiClient apiClient,
        ILogger<UnsplashStockRepository> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<StockImage> GetNextImage(CancellationToken cancellationToken)
    {
        var request = new GetRandomPhoto.Request(
            Topics: _topics,
            Orientation: Orientation.Portrait);

        var response = await _apiClient.GetRandomPhotos(request, cancellationToken);
        var photo = response.Single();

        _logger.LogInformation("Got random photo with id {imageId}", photo.Id);

        return new StockImage(photo.Id, photo.Urls.Raw, photo.Urls.Regular, UnsplashPhotoPagePrefix + photo.Id);
    }

    public async Task MarkAsDownloaded(string imageId, CancellationToken cancellationToken)
    {
        await _apiClient.MarkAsDownloaded(imageId, cancellationToken);
        _logger.LogInformation("Marked as downloaded image with id {imageId}", imageId);
    }
}