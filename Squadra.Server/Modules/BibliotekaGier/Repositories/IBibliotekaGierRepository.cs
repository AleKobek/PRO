using Squadra.Server.Modules.BibliotekaGier.DTO;
using Squadra.Server.Modules.BibliotekaGier.Models;

namespace Squadra.Server.Modules.BibliotekaGier.Repositories;

public interface IBibliotekaGierRepository
{
    public Task<ICollection<GraWBiblioteceDTO>> GetGryWBiblioteceUzytkownika(int idUzytkownika);
    public Task<bool> CzyUzytkownikMaDanaGreNaDanejPlatformie(int idUzytkownika, int idGry, int idPlatformy);
    public Task<bool> CzyUzytkownikMaDanaGre(int idUzytkownika, int idGry);
    public Task<bool> UpdateBibliotekeGierUzytkownika(int idUzytkownika, List<GraUzytkownikaNaPlatformie> noweGryNaPlatformie, List<GraUzytkownika> noweGry);
    public Task<bool> WyczyscBibliotekeUzytkownika(int idUzytkownika);
}