namespace Squadra.Server.DTO.Auth;

public record AuthUserDto(int Id, string Login, string Email, string[] Role);