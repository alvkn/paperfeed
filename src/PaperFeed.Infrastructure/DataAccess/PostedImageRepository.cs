using Microsoft.EntityFrameworkCore;
using PaperFeed.Application.Abstractions.Repositories;
using PaperFeed.Application.Models;

namespace PaperFeed.Infrastructure.DataAccess;

public class PostedImageRepository : IPostedImageRepository
{
    private readonly BotDbContext _dbContext;

    public PostedImageRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> IsAlreadyPosted(string imageId, CancellationToken cancellationToken)
    {
        return await _dbContext.PostedImages.AnyAsync(x => x.Id == imageId, cancellationToken);
    }

    public async Task Add(string imageId, int orderedImageId, CancellationToken cancellationToken)
    {
        _dbContext.PostedImages.Add(new PostedImage
        {
            Id = imageId,
            OrderedId = orderedImageId
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetNextOrderedImageId(CancellationToken cancellationToken)
    {
        var lastPostedImage = await _dbContext.PostedImages
            .OrderByDescending(x => x.OrderedId)
            .FirstOrDefaultAsync(cancellationToken);

        return (lastPostedImage?.OrderedId ?? 0) + 1;
    }
}