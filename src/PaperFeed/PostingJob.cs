﻿using Microsoft.Extensions.Options;
using PaperFeed.Application.Abstractions;

namespace PaperFeed;

public class PostingJob : BackgroundService
{
    private readonly ILogger<PostingJob> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly int _intervalInMinutes;

    public PostingJob(
        ILogger<PostingJob> logger,
        IOptions<BotSettings> botSettings,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _intervalInMinutes = botSettings.Value.IntervalInMinutes;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting background job");

        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Executing scheduled task at {dateTime}", DateTime.UtcNow);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var processingService = scope.ServiceProvider.GetRequiredService<IImagePublisherService>();
                await processingService.PublishNextImage(cancellationToken);
            }

            _logger.LogInformation("Executed task, sleeping for {intervalInMinutes} minutes", _intervalInMinutes);
            await Task.Delay(TimeSpan.FromMinutes(_intervalInMinutes), cancellationToken);
        }
    }
}