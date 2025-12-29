using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Lexy.Tests;

public abstract class ScopedServicesTestFixture
{
    private ServiceProvider instance;
    private IServiceScope serviceScope;

    protected IServiceProvider ServiceProvider
    {
        get
        {
            if (serviceScope == null) throw new InvalidOperationException("ServiceScope not set");
            return serviceScope.ServiceProvider;
        }
    }

    [SetUp]
    public void SetUp()
    {
        var instance = ServiceProviderConfiguration.CreateServices(Initialize);
        serviceScope = instance.CreateScope();
    }

    [TearDown]
    public void TearDown()
    {
        serviceScope?.Dispose();
        serviceScope = null;
    }

    protected T GetService<T>()
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    protected virtual void Initialize(IServiceCollection serviceCollection)
    {
    }
}