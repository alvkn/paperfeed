using Microsoft.Extensions.DependencyInjection;
using PaperFeed.Application.Abstractions;
using PaperFeed.Application.Services;

namespace PaperFeed.Application;

public static class DependencyInjectionExtensions
{
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IImagePublisherService, ImagePublisherService>();

        return services;
    }
}