using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaperFeed.Application.Abstractions;
using PaperFeed.Application.Abstractions.Repositories;
using PaperFeed.Infrastructure.DataAccess;
using PaperFeed.Infrastructure.Http;
using PaperFeed.Infrastructure.Http.Abstractions;
using PaperFeed.Infrastructure.Models.Telegram;
using PaperFeed.Infrastructure.Models.Unsplash;
using PaperFeed.Infrastructure.Publishers;
using PaperFeed.Infrastructure.StockRepositories;
using Telegram.Bot;

namespace PaperFeed.Infrastructure;

public static class DependencyInjectionExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelegramSettings>(configuration.GetSection(nameof(TelegramSettings)));
        services.Configure<UnsplashSettings>(configuration.GetSection(nameof(UnsplashSettings)));

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<TelegramSettings>>().Value;
            var options = new TelegramBotClientOptions(settings.BotToken);
            return new TelegramBotClient(options);
        });

        services.AddHttpClient();
        services.AddHttpClient<IUnsplashApiClient, UnsplashApiClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<UnsplashSettings>>().Value;

            client.BaseAddress = new Uri(options.BaseApiUrl);
            client.DefaultRequestHeaders.Add("Accept-Version", "v1");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", options.AccessKey);
        });

        services.AddHttpClient<IUnsplashHttpScrapper, UnsplashHttpScrapper>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<UnsplashSettings>>().Value;

            client.BaseAddress = new Uri(options.BasePortalUrl);
            client.DefaultRequestHeaders.UserAgent.TryParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36");
        });

        services.AddSingleton<ISocialPublisher, TelegramPublisher>();
        services.AddSingleton<IImageStockRepository, UnsplashStockRepository>();

        services.AddSingleton<IImageDownloader, ImageDownloader>();

        services.AddDbContext<BotDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Default")));

        services.AddScoped<IPostedImageRepository, PostedImageRepository>();

        return services;
    }
}