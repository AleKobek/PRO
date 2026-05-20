using Squadra.Server.Exceptions;
using Squadra.Server.Modules.Drużyny.DTO;
using Squadra.Server.Modules.Shared.Services;
using Squadra.Server.Modules.Statystyki.DTO;
using Squadra.Server.Modules.Statystyki.Models;
using Squadra.Server.Modules.Statystyki.Repositories;

namespace Squadra.Server.Modules.Statystyki.Services;

public class StatystykiService(IStatystykiRepository statystykiRepository) : IStatystykiService
{
    public async Task<ServiceResult<StatystykaDTO>> GetStatystyka(int idStatystyki)
    {
        if (idStatystyki <= 0)
        {
            return ServiceResult<StatystykaDTO>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki: " + idStatystyki));
        }
        
        try
        {
            var result = await statystykiRepository.GetStatystyka(idStatystyki);
            return ServiceResult<StatystykaDTO>.Ok(new StatystykaDTO(idStatystyki, result.Nazwa, result.KategoriaId, result.RolaId, result.CzyToCzasRozgrywki));
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<StatystykaDTO>.NotFound(new ErrorItem(ex.Message));
        }
    }
    public async Task<ServiceResult<string>> GetGodzinyGrania(int idUzytkownika, int idGry)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<string>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        if (idGry <= 0)
        {
            return ServiceResult<string>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator gry: " + idGry));
        }
        
        try
        {
            var result = await statystykiRepository.GetGodzinyGrania(idUzytkownika, idGry);
            return ServiceResult<string>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<string>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<CzasRozgrywkiDTO>>> GetGodzinyGraniaUzytkownika(int idUzytkownika)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<ICollection<CzasRozgrywkiDTO>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        
        try
        {
            var result = await statystykiRepository.GetGodzinyGraniaUzytkownika(idUzytkownika);
            return ServiceResult<ICollection<CzasRozgrywkiDTO>>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<ICollection<CzasRozgrywkiDTO>>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<WartoscStatystykiDTO?>> GetWartoscStatystyki(int idUzytkownika, int idStatystyki)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<WartoscStatystykiDTO?>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        if (idStatystyki <= 0)
        {
            return ServiceResult<WartoscStatystykiDTO?>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki: " + idStatystyki));
        }
        
        try
        {
            var result = await statystykiRepository.GetWartoscStatystyki(idUzytkownika, idStatystyki);
            return ServiceResult<WartoscStatystykiDTO?>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<WartoscStatystykiDTO?>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<StatystykiDoTabelkiDTO>>> GetStatystykiUzytkownikaZGry(int idUzytkownika, int idGry)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        if (idGry <= 0)
        {
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator gry: " + idGry));
        }
        
        try
        {
            var result = await statystykiRepository.GetStatystykiUzytkownikaZGry(idUzytkownika, idGry);
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<ICollection<StatystykiDoTabelkiDTO>>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UpdateStatystykiUzytkownika(int idUzytkownika, List<StatystykaUzytkownika> noweStatystyki)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        
        try
        {
            var result = await statystykiRepository.UpdateStatystykiUzytkownika(idUzytkownika, noweStatystyki);
            return ServiceResult<bool>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(ex.Message));
        }catch(BrakIdNaZewnetrznymSerwisieException e)
        {
            return ServiceResult<bool>.BadRequest(new ErrorItem(e.Message));
        }
    }
    
    public async Task<ServiceResult<bool>> UsunStatystykiUzytkownika(int idUzytkownika)
    {
        if (idUzytkownika <= 0)
        {
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator użytkownika: " + idUzytkownika));
        }
        
        try
        {
            var result = await statystykiRepository.UsunStatystykiUzytkownika(idUzytkownika);
            return ServiceResult<bool>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(ex.Message));
        }
    }

    public ServiceResult<bool> CzySpelniaWymagania(ICollection<WartoscStatystykiDTO> wymagania, ICollection<WartoscStatystykiDTO> statystykiDoSprawdzenia)
    {
        var czySaBledneIdentyfikatoryWWymaganiach = wymagania.Any(x => x.IdStatystyki <= 0);
        if (czySaBledneIdentyfikatoryWWymaganiach){
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki w wymaganiach."));
        }
        var czySaBledneIdentyfikatoryWStatystykachDoSprawdzenia = statystykiDoSprawdzenia.Any(x => x.IdStatystyki <= 0);
        if (czySaBledneIdentyfikatoryWStatystykachDoSprawdzenia){
            return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki w statystykach do sprawdzenia ."));
        }
        var result = statystykiRepository.CzySpelniaWymagania(wymagania, statystykiDoSprawdzenia);
        return ServiceResult<bool>.Ok(result);
    }
    
    public async Task<ServiceResult<ICollection<WymaganieDruzynyDoWyswietleniaDto>>> GetWymaganiaDruzynyDoWyswietlenia(int idDruzyny)
    {
        if (idDruzyny <= 0)
        {
            return ServiceResult<ICollection<WymaganieDruzynyDoWyswietleniaDto>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator drużyny: " + idDruzyny));
        }
        
        try
        {
            var result = await statystykiRepository.GetWymaganiaDruzynyDoWyswietlenia(idDruzyny);
            return ServiceResult<ICollection<WymaganieDruzynyDoWyswietleniaDto>>.Ok(result);
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<ICollection<WymaganieDruzynyDoWyswietleniaDto>>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<ICollection<RolaDto>>> GetRoleGry(int idGry)
    {
        if (idGry <= 0)
        {
            return ServiceResult<ICollection<RolaDto>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator gry: " + idGry));
        }
        
        try
        {
            var result = await statystykiRepository.GetRoleGry(idGry);
            return ServiceResult<ICollection<RolaDto>>.Ok(result.Select(x => new RolaDto(x.Id, x.Nazwa, idGry)).ToList());
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<ICollection<RolaDto>>.NotFound(new ErrorItem(ex.Message));
        }
    }

    // funkcja do zwracania statystyk do formularza. musimy oddzielić rangi od reszty statystyk, bo je się wyświetla inaczej
    public async Task<ServiceResult<StatystykiDoFormularzaDto>> GetStatystykiDoFormularza(int idGry, int idUzytkownika)
    {
        if(idGry <= 0) return ServiceResult<StatystykiDoFormularzaDto>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator gry: " + idGry));
        try
        {            
            var statystyki = await statystykiRepository.GetStatystykiZGry(idGry);
            var wszystkieRangi = await statystykiRepository.GetRangiGry(idGry);
            var rangi = await statystykiRepository.GetMniejszeLubRowneRangiGryUzytkownika(idGry, idUzytkownika);
            var statystykiBezRang = statystyki
                // All zwraca true, jeśli wszystkie elementy spełniają warunek.
                // tutaj sprawdzamy, czy statystyka nie jest rangą, czyli czy nie ma rangi o takim samym id statystyki
                .Where(s => rangi.All(r => r.Id != s.Id))
                .OrderBy(s => s.Id)
                .Select(s => new StatystykaDoFormularzaNieBedacaRangaDto(
                    s.Id,
                    s.RolaId == null
                        ? $"{s.Kategoria.Nazwa}: {s.Nazwa}"
                        : $"{s.Kategoria.Nazwa}: {s.Nazwa}({s.Rola.Nazwa})"
                ))
                .ToList();
            return ServiceResult<StatystykiDoFormularzaDto>.Ok(new StatystykiDoFormularzaDto(statystykiBezRang, rangi, wszystkieRangi));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<StatystykiDoFormularzaDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<ICollection<RolaDto>>> GetRole()
    {
        var role = await statystykiRepository.GetRole();
        return ServiceResult<ICollection<RolaDto>>.Ok(role.Select(x => new RolaDto(x.Id, x.Nazwa, x.IdGry)).ToList());
    }

    public async Task<ServiceResult<ICollection<RangiStatystykiDto>>> GetRangiGry(int idGry)
    {
        if (idGry <= 0)
        {
            return ServiceResult<ICollection<RangiStatystykiDto>>.BadRequest(
                new ErrorItem("Nieprawidłowy identyfikator gry: " + idGry));
        }

        try
        {
            var rangi = await statystykiRepository.GetRangiGry(idGry);
            return ServiceResult<ICollection<RangiStatystykiDto>>.Ok(rangi.Select(x =>
                    new RangiStatystykiDto(x.Id, x.Nazwa,
                        x.Rangi.Select(r => new RangaWDtoRangiStatystykiDto(r.NazwaRangi, r.WartoscLiczbowa)).ToList()))
                .ToList());
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<ICollection<RangiStatystykiDto>>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public async Task<ServiceResult<RolaDto>> GetRola(int idRoli)
    {
        if (idRoli <= 0)
        {
            return ServiceResult<RolaDto>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator roli: " + idRoli));
        }
        
        try
        {
            var rola = await statystykiRepository.GetRola(idRoli);
            return ServiceResult<RolaDto>.Ok(new RolaDto(rola.Id, rola.Nazwa, rola.IdGry));
        }
        catch (NieZnalezionoWBazieException ex)
        {
            return ServiceResult<RolaDto>.NotFound(new ErrorItem(ex.Message));
        }
    }
    
    public ServiceResult<ICollection<int>> FiltrujNieistniejaceStatystyki(ICollection<int> idStatystyk)
    {
        var nieprawidlowaStatystyka = idStatystyk.FirstOrDefault(id => id <= 0);
        if (nieprawidlowaStatystyka != 0)
        {
            return ServiceResult<ICollection<int>>.BadRequest(new ErrorItem("Nieprawidłowy identyfikator statystyki w kolekcji: " + nieprawidlowaStatystyka));
        }
        
        var result =  statystykiRepository.FiltrujNieistniejaceStatystyki(idStatystyk);
        return ServiceResult<ICollection<int>>.Ok(result);
    }
}