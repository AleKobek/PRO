namespace Squadra.Server.Modules.Uzytkownicy.DTO.Auth;

public record LoginRequest(
    string LoginLubEmail, 
    string Haslo, 
    bool ZapamietajMnie
);