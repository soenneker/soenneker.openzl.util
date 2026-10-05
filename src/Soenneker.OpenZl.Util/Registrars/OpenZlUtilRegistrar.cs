using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.OpenZl.Util.Abstract;
namespace Soenneker.OpenZl.Util.Registrars;
/// <summary>Registers the thread-safe OpenZL utility.</summary>
public static class OpenZlUtilRegistrar
{
    /// <summary>Registers IOpenZlUtil as a singleton.</summary>
    public static IServiceCollection AddOpenZlUtilAsSingleton(this IServiceCollection services) { services.TryAddSingleton<IOpenZlUtil, OpenZlUtil>(); return services; }
    /// <summary>Registers IOpenZlUtil for each scope.</summary>
    public static IServiceCollection AddOpenZlUtilAsScoped(this IServiceCollection services) { services.TryAddScoped<IOpenZlUtil, OpenZlUtil>(); return services; }
}
