namespace Squadra.Server.Modules.Znajomosci.DTO;

public record ZnajomiDto(
    int IdUzytkownika1,
    int IdUzytkownika2,
    DateOnly DataNawiazaniaZnajomosci,
    DateTime? OstatnieOtwarcieCzatuUzytkownika1,
    DateTime? OstatnieOtwarcieCzatuUzytkownika2
);