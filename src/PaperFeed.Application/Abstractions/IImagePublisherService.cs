namespace PaperFeed.Application.Abstractions;

public interface IImagePublisherService
{
    Task PublishNextImage(CancellationToken cancellationToken);
}