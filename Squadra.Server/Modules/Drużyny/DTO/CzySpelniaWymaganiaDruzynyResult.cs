namespace Squadra.Server.Modules.Drużyny.DTO;

public record CzySpelniaWymaganiaDruzynyResult(
    bool  CzySpelniaWymagania,  
    string? PowodNiespelnieniaWymagan
);