using Microsoft.EntityFrameworkCore;

namespace Squadra;

public class StatusRepository(AppDbContext context) : IStatusRepository
{
    public async Task<ICollection<StatusDto>> GetStatusy()
    {
        ICollection<StatusDto> statusyDoZwrocenia = new List<StatusDto>();
        ICollection<Status> statusy = await context.Status.ToListAsync();

        foreach (var status in statusy)
        {
            statusyDoZwrocenia.Add(new StatusDto(status.Id, status.Nazwa));
        }
        return statusyDoZwrocenia;
    }

    public async Task<StatusDto?> GetStatus(int id)
    {
        Status? status = await context.Status.FindAsync(id);
        return status != null ? new StatusDto(status.Id, status.Nazwa) : null;
    }

    public async Task<StatusDto?> GetStatus(string nazwa)
    {
        Status? status = await context.Status.Where(x => x.Nazwa == nazwa).FirstOrDefaultAsync();
        return status != null ? new StatusDto(status.Id, status.Nazwa) : null;
    }

    public async Task<int?> GetIdStatusu(string nazwa)
    {
        Status? status = await context.Status.Where(x => x.Nazwa == nazwa).FirstOrDefaultAsync();
        return status?.Id;
    }

    public StatusDto GetStatusDomyslny()
    {
        return new StatusDto(5, "Offline");
    }
}