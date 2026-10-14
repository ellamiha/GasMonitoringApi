using GasMonitoringApi.Data;
using GasMonitoringApi.Dtos;
using GasMonitoringApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GasMonitoringApi.Services;

public class WellService
{
    private readonly AppDbContext db;

    public WellService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<WellResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await db.Wells.AsNoTracking().OrderBy(w => w.Id)
            .Select(w => new WellResponse(w.Id, w.Name, w.MaxPressure, w.PlannedFlow))
            .ToListAsync(cancellationToken);
    }

    public async Task<WellResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var well = await FindAsync(id, cancellationToken);
        return ToResponse(well);
    }

    public async Task<WellResponse> CreateAsync(WellRequest request, CancellationToken cancellationToken)
    {
        var well = new Well
        {
            Name = request.Name.Trim(),
            MaxPressure = request.MaxPressure,
            PlannedFlow = request.PlannedFlow
        };
        db.Wells.Add(well);
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(well);
    }

    public async Task UpdateAsync(int id, WellRequest request, CancellationToken cancellationToken)
    {
        var well = await FindAsync(id, cancellationToken);
        well.Name = request.Name.Trim();
        well.MaxPressure = request.MaxPressure;
        well.PlannedFlow = request.PlannedFlow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var well = await FindAsync(id, cancellationToken);
        if (await db.Measurements.AnyAsync(m => m.WellId == id, cancellationToken))
            throw new ServiceException(ServiceError.Conflict, "Сначала удалите измерения этой скважины.");
        db.Wells.Remove(well);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Well> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await db.Wells.FirstOrDefaultAsync(w => w.Id == id, cancellationToken)
            ?? throw new ServiceException(ServiceError.NotFound, "Скважина не найдена.");
    }

    private static WellResponse ToResponse(Well well)
    {
        return new WellResponse(well.Id, well.Name, well.MaxPressure, well.PlannedFlow);
    }
}
