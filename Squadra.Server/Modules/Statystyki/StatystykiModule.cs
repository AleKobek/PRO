using Squadra.Server.Modules.Statystyki.Repositories;
using Squadra.Server.Modules.Statystyki.Services;

namespace Squadra.Server.Modules.Statystyki;

public static class StatystykiModule
{
    public static IServiceCollection AddStatystykiModule(this IServiceCollection services)
    {
        services.AddScoped<IStatystykiRepository, StatystykiRepository>();
        services.AddScoped<IStatystykiService, StatystykiService>();
        
        return services;
    }
}