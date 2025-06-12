namespace PaperFeed.Application.Models.PostImages;

public class PreviewImage : PostImage
{
    public required string Caption { get; init; }

    public required string Id { get; init; }

    public required string StockPageUrl { get; init; }

    public required string PreviewUrl { get; init; }

    public bool CaptionAsLink { get; init; } = true;
}
