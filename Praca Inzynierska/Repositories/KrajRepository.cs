using Microsoft.EntityFrameworkCore;
using Praca_Inzynierska.Context;
using Praca_Inzynierska.DTO;
using Praca_Inzynierska.Models;

namespace Praca_Inzynierska.Repositories;

public class KrajRepository : IKrajRepository
{
    private readonly AppDbContext _context;

    public KrajRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }


    public async Task<ICollection<KrajDto>> GetKraje()
    {
        ICollection<KrajDto> krajeDoZwrocenia = new List<KrajDto>();
        ICollection<Kraj> kraje = await _context.Kraj.ToListAsync();

        foreach (Kraj kraj in kraje)
        {
            krajeDoZwrocenia.Add(new KrajDto(kraj.Id, kraj.Nazwa));
        }

        return krajeDoZwrocenia;
    }

    public async Task<KrajDto?> GetKraj(int id)
    {
        Kraj? kraj = await _context.Kraj.FindAsync(id);
        
        if (kraj == null) return null;
        
        return new KrajDto(kraj.Id, kraj.Nazwa);
    }
}