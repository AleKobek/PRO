using Squadra.Server.DTO.Profil;
using Squadra.Server.DTO.Status;
using Squadra.Server.Exceptions;
using Squadra.Server.Repositories;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Squadra.Server.Services;

public class ProfilService(
    IProfilRepository profilRepository, 
    IUzytkownikRepository uzytkownikRepository,
    IStatusRepository statusRepository) : IProfilService
{
    
    public async Task<ServiceResult<ICollection<ProfilGetResDto>>> GetProfile()
    {
        return ServiceResult<ICollection<ProfilGetResDto>>.Ok(await profilRepository.GetProfile());
    }

    public async Task<ServiceResult<ProfilGetResDto>> GetProfil(int id)
    {
        try{
            return id < 1
                ? ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem("Profil o id " + id + " nie istnieje"))
                : ServiceResult<ProfilGetResDto>.Ok(await profilRepository.GetProfilUzytkownika(id));
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<ProfilGetResDto>> GetProfil(string login)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem("Nieprawidłowy login"));
            }
            var uzytkownik = await uzytkownikRepository.GetUzytkownik(login);
            return ServiceResult<ProfilGetResDto>.Ok(await profilRepository.GetProfilUzytkownika(uzytkownik.Id));
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<ProfilGetResDto>.NotFound(new ErrorItem(e.Message));
        }
    }
    
    
    public async Task<ServiceResult<bool>> UpdateProfil(int id, ProfilUpdateDto profil)
    {
        /*
            dane przyjdą w formie:
            int IdUzytkownika,
            int RegionId,
            string? Zaimki,
            string? Opis,
            ICollection<JezykOrazStopienDto> Jezyki,
            string Pseudonim,
            byte[]? Awatar
        */
        
        var bledy = new List<ErrorItem>();
        
        if(id < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Profil o id " + id + " nie istnieje"));
        if(profil.RegionId < 1) return ServiceResult<bool>.BadRequest(new ErrorItem("Region o id " + profil.RegionId + " nie istnieje"));
        
        if(profil.Zaimki is { Length: > 10 })
        {
            bledy.Add(new ErrorItem("Maksymalna długość zaimków wynosi 10 znaków", nameof(profil.Zaimki)));
        }

        if (profil.Opis is { Length: > 100 })
        {
            bledy.Add(new ErrorItem("Maksymalna długość opisu wynosi 100 znaków", nameof(profil.Opis)));
        }

        if (profil.Pseudonim.Length > 20)
        {
            bledy.Add(new ErrorItem("Maksymalna długość pseudonimu wynosi 20 znaków", nameof(profil.Pseudonim)));
        }
        
        try
        {
            return bledy.Count > 0
                ? ServiceResult<bool>.BadRequest(bledy.ToArray())
                : ServiceResult<bool>.Ok(await profilRepository.UpdateProfil(id, profil), 204);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<bool>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<bool>> UpdateAwatar(int id, IFormFile awatar)
    {
        if(awatar.Length == 0) return ServiceResult<bool>.BadRequest(new ErrorItem("Nieprawidłowy awatar"));
        using (var memoryStream = new MemoryStream())
        {
            // przenosimy to do memory stream
            await awatar.CopyToAsync(memoryStream);
            // przenosimy to do tablicy bajtów, tak jak chcemy to mieć
            var awatarWBajtach = memoryStream.ToArray();
            awatarWBajtach = NormalizujAwatar(awatarWBajtach);
            return ServiceResult<bool>.Ok(await profilRepository.UpdateAwatar(id, awatarWBajtach ?? []),204);
        }
    }
    public async Task<ServiceResult<StatusDto>> GetStatusZBazyProfilu(int id)
    {
        try{
            return id < 1
                ? ServiceResult<StatusDto>.BadRequest(new ErrorItem("Profil o id " + id + " nie istnieje"))
                : ServiceResult<StatusDto>.Ok(await profilRepository.GetStatusProfilu(id));
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<StatusDto>> GetStatusDoWyswietleniaProfilu(int id)
    {
        if(id < 1) return ServiceResult<StatusDto>.NotFound(new ErrorItem("Profil o id " + id + " nie istnieje"));

        try
        {
            var status = await profilRepository.GetStatusProfilu(id);

            var aktywnosc = await uzytkownikRepository.GetOstatniaAktywnoscUzytkownika(id);
            
            // uznajemy, że jest offline, jeżeli nie miał otwartego okna / połączenia przez 5 minut
            return ServiceResult<StatusDto>.Ok(DateTime.Now - aktywnosc > TimeSpan.FromMinutes(5) || status.Nazwa == "Niewidoczny" 
                ? statusRepository.GetStatusOffline()
                : status);
        }
        catch (NieZnalezionoWBazieException e)
        {
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    public async Task<ServiceResult<StatusDto>> UpdateStatus(int id, int idStatus)
    {
        try{
            if (id < 1)
                return ServiceResult<StatusDto>.NotFound(new ErrorItem("Profil o id " + id + " nie istnieje"));

            if (idStatus < 1)
                return ServiceResult<StatusDto>.NotFound(new ErrorItem("Status o id " + id + " nie istnieje"));

            return ServiceResult<StatusDto>.Ok(await profilRepository.UpdateStatus(id, idStatus));
            
        }catch(NieZnalezionoWBazieException e){
            return ServiceResult<StatusDto>.NotFound(new ErrorItem(e.Message));
        }
    }

    // awatar ma zawsze być JPEG o rozmiarze 256 x 256
    private byte[]? NormalizujAwatar(byte[]? awatar)
    {
        if(awatar == null || awatar.Length == 0) return null;
        
        using var image = Image.Load(awatar); // wykrywa PNG/JPG/WEBP automatycznie, jak to nie jest obraz rzuci wyjątkiem

        // Skalowanie do kwadratu np. 256x256 (zachowa proporcje)
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(256, 256),
            Mode = ResizeMode.Crop // wypełnia cały kwadrat
        }));

        // Konwersja do JPEG
        using var ms = new MemoryStream();
        image.Save(ms, new JpegEncoder
        {
            Quality = 85 // 70–90 to najlepszy kompromis
        });

        return ms.ToArray();
    }
}