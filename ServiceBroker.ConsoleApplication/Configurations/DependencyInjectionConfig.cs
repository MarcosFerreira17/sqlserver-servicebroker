using Microsoft.Extensions.DependencyInjection;
using ServiceBroker.ConsoleApplication.Interfaces;
using ServiceBroker.ConsoleApplication.Services;

namespace ServiceBroker.ConsoleApplication.Configurations;
public static class DependencyInjectionConfig
{
    public static IServiceCollection ResolveDependencies(this IServiceCollection services)
    {
        services.AddScoped<IPublisherService, PublisherService>();
        services.AddScoped<ISubscribeService, SubscriberService>();
        services.AddHostedService<HostBackgroundService>();

        return services;
    }
}
