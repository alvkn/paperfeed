using Microsoft.Extensions.Logging;
using PaperFeed.Application.Abstractions;
using PaperFeed.Application.Abstractions.Repositories;
using PaperFeed.Application.Models;
using PaperFeed.Application.Models.PostImages;
using Polly;

namespace PaperFeed.Application.Services;

public class ImagePublisherService : IImagePublisherService
{
    private readonly ISocialPublisher _socialPublisher;
    private readonly IImageStockRepository _imageStockRepository;
    private readonly IPostedImageRepository _postedImageRepository;
    private readonly IImageDownloader _imageDownloader;
    private readonly ILogger<ImagePublisherService> _logger;

    public ImagePublisherService(
        ILogger<ImagePublisherService> logger,
        IImageStockRepository imageStockRepository,
        IPostedImageRepository postedImageRepository,
        IImageDownloader imageDownloader,
        ISocialPublisher socialPublisher)
    {
        _logger = logger;
        _imageStockRepository = imageStockRepository;
        _postedImageRepository = postedImageRepository;
        _imageDownloader = imageDownloader;
        _socialPublisher = socialPublisher;
    }

    public async Task PublishNextImage(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to create and publish a new post.");

            var sourceImage = await TryGetNextImage(cancellationToken);
            if (sourceImage is null)
            {
                _logger.LogWarning("No new suitable image found from the source.");
                return;
            }

            var orderedId = await _postedImageRepository.GetNextOrderedImageId(cancellationToken);
            var imageFile = await _imageDownloader.Download(sourceImage.SourceUrl, cancellationToken);

            var postImages = new PostImage[]
            {
                new PreviewImage
                {
                    Caption = "source",
                    Id = sourceImage.Id,
                    StockPageUrl = sourceImage.StockPageUrl,
                    Content = imageFile.Content
                },
                new OriginalImage
                {
                    FileName = $"paperfeed_{orderedId}.{imageFile.FileExtension}",
                    Content = imageFile.Content
                }
            };

            await _socialPublisher.Publish(postImages, cancellationToken);
            await _imageStockRepository.MarkAsDownloaded(sourceImage.Id, cancellationToken);
            await _postedImageRepository.Add(sourceImage.Id, orderedId, cancellationToken);

            _logger.LogInformation("Successfully published post for image {imageId} with ordered id {orderedId}",
                sourceImage.Id,
                orderedId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish post for image");
        }
    }

    private async Task<StockImage?> TryGetNextImage(CancellationToken cancellationToken)
    {
        const int maxAttempts = 10;

        var policy = Policy
            .HandleResult<StockImage?>(image => image == null)
            .RetryAsync(maxAttempts - 1);

        var result = await policy.ExecuteAsync(async ct =>
            {
                var candidate = await _imageStockRepository.GetNextImage(ct);

                if (await _postedImageRepository.IsAlreadyPosted(candidate.Id, ct))
                {
                    _logger.LogInformation("Photo with image id {imageId} is already posted", candidate.Id);
                    return null;
                }

                return candidate;
            },
            cancellationToken);

        return result;
    }
}