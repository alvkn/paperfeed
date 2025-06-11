using System.Text.Json;
using System.Text.Json.Serialization;
using PaperFeed;
using PaperFeed.Application;
using PaperFeed.Infrastructure;
using PaperFeed.Infrastructure.Models.Unsplash.Api.Endpoints;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<BotSettings>(builder.Configuration.GetSection(nameof(BotSettings)));

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    Converters =
    {
        new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower),
        new SingleOrArrayConverter<GetRandomPhoto.Request>()
    }
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<PostingJob>();

await builder.Build().RunAsync();