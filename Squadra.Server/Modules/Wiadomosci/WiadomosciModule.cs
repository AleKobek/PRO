namespace Squadra.Server.Modules.Wiadomosci;
using Repositories;
using Services;

public static class WiadomosciModule
{
    public static IServiceCollection AddWiadomosciModule(this IServiceCollection services)
    {
        services.AddScoped<IWiadomoscRepository, WiadomoscRepository>();
        services.AddScoped<IWiadomoscService, WiadomoscService>();
        return services;
    }

}