using Microsoft.Extensions.DependencyInjection;
using Nergard.MissingProperties.Services;

namespace Nergard.MissingProperties;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Missing Properties tool services.
    /// </summary>
    public static IServiceCollection AddMissingProperties(this IServiceCollection services)
    {
        services.AddScoped<IMissingPropertyService, MissingPropertyService>();
        return services;
    }
}
