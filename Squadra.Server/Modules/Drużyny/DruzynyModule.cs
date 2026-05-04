using Squadra.Server.Modules.Drużyny.Repositories;
using Squadra.Server.Modules.Drużyny.Services;

namespace Squadra.Server.Modules.Drużyny;

public static class DruzynyModule
{
    public static IServiceCollection AddDruzynyModule(this IServiceCollection services)
    {
        services.AddScoped<IDruzynyService, DruzynyService>();
        services.AddScoped<IDruzynyRepository, DruzynyRepository>();
        
        return services;
    }
}