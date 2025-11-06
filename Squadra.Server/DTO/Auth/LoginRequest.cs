namespace Squadra.Server.DTO.Auth;

public record LoginRequest(
    string LoginLubEmail, 
    string Haslo, 
    bool ZapamietajMnie
);