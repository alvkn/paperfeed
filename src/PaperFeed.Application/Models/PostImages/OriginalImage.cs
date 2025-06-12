namespace PaperFeed.Application.Models.PostImages;

public class OriginalImage : PostImage
{
    public required string FileName { get; init; }

    public required byte[] Content { get; init; }
}