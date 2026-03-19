using Squadra.Server.Modules.Platformy.Repositories;
using Squadra.Server.Modules.Platformy.Services;

namespace Squadra.Server.Modules.Platformy;

public static class PlatformaModule
{
    public static IServiceCollection AddPlatformaModule(this IServiceCollection services)
    {
        services.AddScoped<IPlatformaRepository, PlatformaRepository>();
        services.AddScoped<IPlatformaService, PlatformaService>();
        
        services.AddScoped<IUzytkownikPlatformaRepository, UzytkownikPlatformaRepository>();
        services.AddScoped<IUzytkownikPlatformaService, UzytkownikPlatformaService>();
        return services;
    }
}