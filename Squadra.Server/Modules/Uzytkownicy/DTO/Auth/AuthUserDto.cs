namespace Squadra.Server.Modules.Uzytkownicy.DTO.Auth;

public record AuthUserDto(
    int Id, 
    string Login, 
    string Email, 
    string[] Role,
    byte[]? Awatar);