namespace Squadra.Server.Modules.Profil;
using Repositories;
using Services;

public static class ProfilModule
{
    public static IServiceCollection AddProfilModule(this IServiceCollection services)
    {
        services.AddScoped<IProfilService, ProfilService>();
        services.AddScoped<IProfilRepository, ProfilRepository>();
        
        services.AddScoped<IKrajRepository, KrajRepository>();
        services.AddScoped<IKrajService, KrajService>();
        
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<IStatusService, StatusService>();
        
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IRegionService, RegionService>();

        services.AddScoped<IJezykRepository, JezykRepository>();
        services.AddScoped<IJezykService, JezykService>();

        services.AddScoped<IStopienBieglosciJezykaRepository, StopienBieglosciJezykaRepository>();
        services.AddScoped<IStopienBieglosciJezykaService, StopienBieglosciJezykaService>();

        
        return services;
    }

}