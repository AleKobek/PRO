using Squadra.Server.Modules.Platformy.Repositories;
using Squadra.Server.Modules.Platformy.Services;

namespace Squadra.Server.Modules.Platformy;

public static class PlatformaModule
{
    public static IServiceCollection AddPlatformaModule(this IServiceCollection services)
    {
        services.AddScoped<IPlatformyRepository, PlatformyRepository>();
        services.AddScoped<IPlatformyService, PlatformyService>();
        
        return services;
    }
}