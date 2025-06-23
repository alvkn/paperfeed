namespace PaperFeed.Infrastructure.Models.Unsplash;

public class UnsplashSettings
{
    public required string AccessKey { get; init; }

    public required string BaseApiUrl { get; init; }

    public required string BasePortalUrl { get; init; }
}