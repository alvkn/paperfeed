using PaperFeed.Application.Models;

namespace PaperFeed.Application.Abstractions.Repositories;

public interface IImageStockRepository
{
    Task<StockImage> GetNextImage(CancellationToken cancellationToken);

    // Some image sources require marking image as downloaded when your app actually downloads the file
    // We want to respect both their API guidelines and author's work
    Task MarkAsDownloaded(string imageId, CancellationToken cancellationToken);
}