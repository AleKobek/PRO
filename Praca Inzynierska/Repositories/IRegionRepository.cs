using Praca_Inzynierska.DTO;

namespace Praca_Inzynierska.Repositories;

public interface IRegionRepository
{
    public Task<ICollection<RegionDto>> GetRegiony();

    public Task<RegionDto?> GetRegion(int id);

    public Task<ICollection<RegionDto>> GetRegionyKraju(int krajId);

}