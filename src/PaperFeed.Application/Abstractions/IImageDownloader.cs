using PaperFeed.Application.Models;

namespace PaperFeed.Application.Abstractions;

public interface IImageDownloader
{
    Task<ImageFile> Download(string imageUrl, CancellationToken cancellationToken);
}
