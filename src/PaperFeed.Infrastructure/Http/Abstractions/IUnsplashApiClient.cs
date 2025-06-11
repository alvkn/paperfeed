using PaperFeed.Infrastructure.Models.Unsplash.Api.Endpoints;

namespace PaperFeed.Infrastructure.Http.Abstractions;

public interface IUnsplashApiClient
{
    Task<IEnumerable<GetRandomPhoto.Response>> GetRandomPhotos(
        GetRandomPhoto.Request request,
        CancellationToken cancellationToken);

    Task MarkAsDownloaded(
        string imageId,
        CancellationToken cancellationToken);
}