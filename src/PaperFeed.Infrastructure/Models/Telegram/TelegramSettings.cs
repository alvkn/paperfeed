namespace PaperFeed.Infrastructure.Models.Telegram;

public class TelegramSettings
{
    public required string BotToken { get; init; }

    public long ChannelId { get; init; }

    public int LocalApiServerPort { get; init; }

    public bool UseOfficialApi { get; init; }

}