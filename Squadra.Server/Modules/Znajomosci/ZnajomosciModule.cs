using Squadra.Server.Modules.Znajomosci.Repositories;
using Squadra.Server.Modules.Znajomosci.Services;

namespace Squadra.Server.Modules.Znajomosci;

public static class ZnajomosciModule
{
    public static IServiceCollection AddZnajomosciModule(this IServiceCollection services)
    {
        services.AddScoped<IZnajomosciService, ZnajomosciService>();
        services.AddScoped<IZnajomosciRepository, ZnajomosciRepository>();
        services.AddScoped<IDeleteZnajomoscService, DeleteZnajomoscService>();

        return services;
    }

}