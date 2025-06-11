namespace PaperFeed.Infrastructure;

public static class StreamExtensions
{
    public static async Task<byte[]> ToByteArray(this Stream stream)
    {
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}