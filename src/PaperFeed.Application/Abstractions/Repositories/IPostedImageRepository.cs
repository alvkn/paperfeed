namespace PaperFeed.Application.Abstractions.Repositories;

public interface IPostedImageRepository
{
    Task<bool> IsAlreadyPosted(string imageId, CancellationToken cancellationToken);

    Task Add(string imageId, int orderedImageId, CancellationToken cancellationToken);

    Task<int> GetNextOrderedImageId(CancellationToken cancellationToken);
}