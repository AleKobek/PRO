namespace Squadra.Server.Modules.Znajomi;
using Repositories;
using Services;

public static class ZnajomiModule
{
    public static IServiceCollection AddZnajomiModule(this IServiceCollection services)
    {
        services.AddScoped<IZnajomiService, ZnajomiService>();
        services.AddScoped<IZnajomiRepository, ZnajomiRepository>();

        return services;
    }

}