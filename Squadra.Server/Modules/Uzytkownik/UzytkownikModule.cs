namespace Squadra.Server.Modules.Uzytkownik;
using Repositories;
using Services;

public static class UzytkownikModule
{
    public static IServiceCollection AddUzytkownikModule(this IServiceCollection services)
    {
        services.AddScoped<IUzytkownikService, UzytkownikService>();
        services.AddScoped<IUzytkownikRepository, UzytkownikRepository>();
        
        return services;
    }
}