using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Application layer services with the DI container.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR - auto-discovers all handlers in this assembly
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
