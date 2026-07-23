using Squadra.Server.Modules.Uzytkownicy.Repositories;
using Squadra.Server.Modules.Uzytkownicy.Services;

namespace Squadra.Server.Modules.Uzytkownicy;

public static class UzytkownicyModule
{
    public static IServiceCollection AddUzytkownicyModule(this IServiceCollection services)
    {
        services.AddScoped<IUzytkownicyService, UzytkownicyService>();
        services.AddScoped<IUzytkownicyRepository, UzytkownicyRepository>();
        
        services.AddScoped<IUsunKontoService, UsunKontoService>();
        
        return services;
    }
}