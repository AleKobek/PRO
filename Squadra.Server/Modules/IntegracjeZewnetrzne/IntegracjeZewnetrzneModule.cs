using Squadra.Server.Modules.IntegracjeZewnetrzne.Repositories;
using Squadra.Server.Modules.IntegracjeZewnetrzne.Services;

namespace Squadra.Server.Modules.IntegracjeZewnetrzne;

public static class IntegracjeZewnetrzneModule
{
    public static IServiceCollection AddIntegracjeZewnetrzneModule(this IServiceCollection services)
    {
        services.AddScoped<IIntegracjeZewnetrzneRepository, IntegracjeZewnetrzneRepository>();
        services.AddScoped<IIntegracjeZewnetrzneService, IntegracjeZewnetrzneService>();
        
        return services;
    }
}