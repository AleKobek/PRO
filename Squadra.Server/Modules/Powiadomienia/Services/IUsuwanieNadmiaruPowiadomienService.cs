namespace Squadra.Server.Modules.Powiadomienia.Services;

public interface IUsuwanieNadmiaruPowiadomienService
{
    public Task<bool> UsunNadmiarowePowiadomieniaUzytkownika(int idUzytkownika);
}