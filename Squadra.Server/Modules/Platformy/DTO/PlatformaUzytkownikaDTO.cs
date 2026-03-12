namespace Squadra.Server.Modules.Platformy.DTO;

public record PlatformaUzytkownikaDTO(
    int IdPlatformy,
    string Nazwa,
    byte[] Logo,
    string PseudonimNaPlatformie
);