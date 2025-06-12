using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaperFeed.Application.Abstractions;
using PaperFeed.Application.Models.PostImages;
using PaperFeed.Infrastructure.Models.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PaperFeed.Infrastructure.Publishers;

public class TelegramPublisher : ISocialPublisher
{
    private readonly ITelegramBotClient _botClient;
    private readonly IOptions<TelegramSettings> _settings;
    private readonly ILogger<TelegramPublisher> _logger;

    public TelegramPublisher(
        ITelegramBotClient botClient,
        IOptions<TelegramSettings> settings,
        ILogger<TelegramPublisher> logger)
    {
        _botClient = botClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task Publish(IEnumerable<PostImage> postImages, CancellationToken token)
    {
        foreach (var postImage in postImages)
        {
            switch (postImage)
            {
                case PreviewImage previewImage:
                    await SendImageAsPhoto(
                        previewImage.PreviewUrl,
                        previewImage.StockPageUrl,
                        previewImage.Caption,
                        previewImage.CaptionAsLink);
                    break;
                case OriginalImage originalImage:
                    await SendImageAsDocument(originalImage.Content, originalImage.FileName);
                    break;
            }
        }
    }

    private async Task SendImageAsPhoto(
        string previewUrl,
        string stockPageUrl,
        string caption,
        bool captionAsLink)
    {
        if (captionAsLink)
        {
            caption = $"[{caption}]({stockPageUrl})";
        }

        await _botClient.SendPhoto(
            chatId: new ChatId(_settings.Value.ChannelId),
            photo: InputFile.FromUri(previewUrl),
            caption: caption,
            parseMode: ParseMode.MarkdownV2);

        _logger.LogInformation("Sent image as photo {stockPageUrl}", stockPageUrl);
    }

    private async Task SendImageAsDocument(byte[] imageBytes, string fileName)
    {
        using var memoryStream = new MemoryStream(imageBytes);

        await _botClient.SendDocument(
            chatId: new ChatId(_settings.Value.ChannelId),
            document: InputFile.FromStream(memoryStream, fileName));

        _logger.LogInformation("Sent image as document: {fileName}", fileName);
    }
}