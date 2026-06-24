namespace Squadra.Server.Modules.Powiadomienia;
using Repositories;
using Services;

public static class PowiadomieniaModule
{
    public static IServiceCollection AddPowiadomieniaModule(this IServiceCollection services)
    {
        services.AddScoped<IPowiadomienieService, PowiadomienieService>();
        services.AddScoped<IPowiadomienieRepository, PowiadomienieRepository>();
        services.AddScoped<IRozpatrzPowiadomienieService, RozpatrzPowiadomienieService>();
        return services;
    }
}