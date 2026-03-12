using Squadra.Server.Modules.Uzytkownicy.Repositories;
using Squadra.Server.Modules.Uzytkownicy.Services;

namespace Squadra.Server.Modules.Uzytkownicy;

public static class UzytkownicyModule
{
    public static IServiceCollection AddUzytkownicyModule(this IServiceCollection services)
    {
        services.AddScoped<IUzytkownikService, UzytkownikService>();
        services.AddScoped<IUzytkownikRepository, UzytkownikRepository>();
        
        return services;
    }
}