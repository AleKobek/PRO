using Squadra.Server.Modules.WspieraneGry.Repositories;
using Squadra.Server.Modules.WspieraneGry.Services;

namespace Squadra.Server.Modules.WspieraneGry;

public static class WspieraneGryModule
{
    public static IServiceCollection AddWspieraneGryModule(this IServiceCollection services)
    {
        services.AddScoped<IWspieraneGryRepository, WspieraneGryRepository>();
        services.AddScoped<IWspieraneGryService, WspieraneGryService>();
        return services;
    }
}