using PaperFeed.Application.Abstractions;
using PaperFeed.Application.Models;

namespace PaperFeed.Infrastructure.Http;

public class ImageDownloader : IImageDownloader
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly Dictionary<string, string> ImageContentTypeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = "jpg",
        ["image/png"] = "png",
        ["image/gif"] = "gif",
        ["image/webp"] = "webp",
        ["image/bmp"] = "bmp",
        ["image/heif"] = "heif",
        ["image/heic"] = "heic"
    };

    public ImageDownloader(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ImageFile> Download(string imageUrl, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync(imageUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        var fileExtension = ImageContentTypeExtensions!.GetValueOrDefault(mediaType, ".jpg");

        await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var content = await responseStream.ToByteArray();

        return new ImageFile(content, fileExtension);
    }
}