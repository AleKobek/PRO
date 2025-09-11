using Microsoft.EntityFrameworkCore;


namespace Squadra;

public class KrajRepository(AppDbContext context) : IKrajRepository
{
    public async Task<ICollection<KrajDto>> GetKraje()
    {
        ICollection<KrajDto> krajeDoZwrocenia = new List<KrajDto>();
        ICollection<Kraj> kraje = await context.Kraj.ToListAsync();

        foreach (Kraj kraj in kraje)
        {
            krajeDoZwrocenia.Add(new KrajDto(kraj.Id, kraj.Nazwa));
        }

        return krajeDoZwrocenia;
    }

    public async Task<KrajDto?> GetKraj(int id)
    {
        Kraj? kraj = await context.Kraj.FindAsync(id);
        
        if (kraj == null) return null;
        
        return new KrajDto(kraj.Id, kraj.Nazwa);
    }
    
    public KrajDto GetKrajDomyslny()
    {
        return new KrajDto(1, "Unknown");
    }
}