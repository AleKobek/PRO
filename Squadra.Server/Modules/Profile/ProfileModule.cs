using Squadra.Server.Modules.Profile.Repositories;
using Squadra.Server.Modules.Profile.Services;

namespace Squadra.Server.Modules.Profile;

public static class ProfileModule
{
    public static IServiceCollection AddProfileModule(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        
        services.AddScoped<IKrajeRepository, KrajeRepository>();
        services.AddScoped<IKrajeService, KrajeService>();
        
        services.AddScoped<IStatusyRepository, StatusyRepository>();
        services.AddScoped<IStatusyService, StatusyService>();
        
        services.AddScoped<IRegionyRepository, RegionyRepository>();
        services.AddScoped<IRegionyService, RegionyService>();

        services.AddScoped<IJezykiRepository, JezykiRepository>();
        services.AddScoped<IJezykiService, JezykiService>();

        services.AddScoped<IStopnieBieglosciJezykaRepository, StopnieBieglosciJezykaRepository>();
        services.AddScoped<IStopnieBieglosciJezykaService, StopnieBieglosciJezykaService>();

        
        return services;
    }

}