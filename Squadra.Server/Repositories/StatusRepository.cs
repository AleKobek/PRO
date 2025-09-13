using Microsoft.EntityFrameworkCore;

namespace Squadra;

public class StatusRepository(AppDbContext context) : IStatusRepository
{
    public async Task<ICollection<StatusDto>> GetStatusy()
    {
        ICollection<Status> statusy = await context.Status.ToListAsync();

        return statusy.Select(status => new StatusDto(status.Id, status.Nazwa)).ToList();
    }

    public async Task<StatusDto?> GetStatus(int id)
    {
        var status = await context.Status.FindAsync(id);
        return status != null ? new StatusDto(status.Id, status.Nazwa) : null;
    }

    public async Task<StatusDto?> GetStatus(string nazwa)
    {
        var status = await context.Status.Where(x => x.Nazwa == nazwa).FirstOrDefaultAsync();
        return status != null ? new StatusDto(status.Id, status.Nazwa) : null;
    }

    public async Task<int?> GetIdStatusu(string nazwa)
    {
        var status = await context.Status.Where(x => x.Nazwa == nazwa).FirstOrDefaultAsync();
        return status?.Id;
    }

    public StatusDto GetStatusDomyslny()
    {
        return new StatusDto(5, "Offline");
    }
}