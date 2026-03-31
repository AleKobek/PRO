using Squadra.Server.Modules.BibliotekaGier.Repositories;
using Squadra.Server.Modules.BibliotekaGier.Services;

namespace Squadra.Server.Modules.BibliotekaGier;

public static class BibliotekaGierModule
{
    public static IServiceCollection AddBibliotekaGierModule(this IServiceCollection services)
    {
        services.AddScoped<IBibliotekaGierRepository, BibliotekaGierRepository>();
        services.AddScoped<IBibliotekaGierService, BibliotekaGierService>();
        
        return services;
    }
}