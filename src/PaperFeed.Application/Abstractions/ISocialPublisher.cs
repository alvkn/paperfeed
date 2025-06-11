using PaperFeed.Application.Models.PostImages;

namespace PaperFeed.Application.Abstractions;

public interface ISocialPublisher
{
    Task Publish(IEnumerable<PostImage> postImages, CancellationToken token);
}