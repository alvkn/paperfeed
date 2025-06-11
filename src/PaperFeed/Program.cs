using System.Text.Json;
using System.Text.Json.Serialization;
using PaperFeed;
using PaperFeed.Application;
using PaperFeed.Infrastructure;
using PaperFeed.Infrastructure.Models.Unsplash.Api.Endpoints;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.Configure<BotSettings>(configuration.GetSection(nameof(BotSettings)));

        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower),
                new SingleOrArrayConverter<GetRandomPhoto.Request>()
            }
        });

        services
            .AddApplication()
            .AddInfrastructure(configuration);

        services.AddHostedService<PostingJob>();
    })
    .Build();

await host.RunAsync();