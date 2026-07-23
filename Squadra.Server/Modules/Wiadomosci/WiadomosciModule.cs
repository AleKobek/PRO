namespace Squadra.Server.Modules.Wiadomosci;
using Repositories;
using Services;

public static class WiadomosciModule
{
    public static IServiceCollection AddWiadomosciModule(this IServiceCollection services)
    {
        services.AddScoped<IWiadomosciRepository, WiadomosciRepository>();
        services.AddScoped<IWiadomosciService, WiadomosciService>();
        services.AddScoped<IStatystykiCzatuService, StatystykiCzatuService>();
        return services;
    }

}