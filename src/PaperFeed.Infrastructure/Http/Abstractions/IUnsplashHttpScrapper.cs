namespace PaperFeed.Infrastructure.Http.Abstractions;

public interface IUnsplashHttpScrapper
{
    Task<IEnumerable<string>> GetPhotoTags(string imageId, CancellationToken cancellationToken);
}