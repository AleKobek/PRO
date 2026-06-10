namespace Squadra.Server.Modules.Drużyny.DTO;

public record MiejsceWDruzynieDto(
   int Id,
   int DruzynaId,
   int? UzytkownikId,
   int? RolaId,
   int? StatystykaId,
   string? WartoscStatystyki,
   double? WartoscLiczbowaStatystyki
);