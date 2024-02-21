using LightValidation.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LightValidation.Microsoft.DependencyInjection.Internal;

internal sealed class DependencyResolver : IDependencyResolver
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TService GetService<TService>()
    {
        return (TService)_serviceProvider.GetRequiredService(typeof(TService));
    }
}
