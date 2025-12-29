using System;
using Lexy.Compiler;
using Microsoft.Extensions.DependencyInjection;

namespace Lexy.Tests;

public static class ServiceProviderConfiguration
{
    public static ServiceProvider CreateServices(Action<IServiceCollection> configuration)
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(LoggingConfiguration.CreateLoggerFactory())
            .AddLexy();

        configuration(serviceProvider);

        return serviceProvider.BuildServiceProvider();
    }
}