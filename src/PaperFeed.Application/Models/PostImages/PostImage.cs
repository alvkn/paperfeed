namespace PaperFeed.Application.Models.PostImages;

public abstract class PostImage
{
    public required byte[] Content { get; init; }
}