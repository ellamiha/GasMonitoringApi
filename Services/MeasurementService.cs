using GasMonitoringApi.Data;
using GasMonitoringApi.Dtos;
using GasMonitoringApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GasMonitoringApi.Services;

public class MeasurementService
{
    private readonly AppDbContext db;

    public MeasurementService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<MeasurementResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await db.Measurements.AsNoTracking().OrderBy(m => m.Id)
            .Select(m => new MeasurementResponse(m.Id, m.WellId, m.RecordedAt, m.Pressure, m.Flow))
            .ToListAsync(cancellationToken);
    }

    public async Task<MeasurementResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var measurement = await FindAsync(id, cancellationToken);
        return ToResponse(measurement);
    }

    public async Task<MeasurementResponse> CreateAsync(MeasurementRequest request, CancellationToken cancellationToken)
    {
        await CheckWellAsync(request.WellId, cancellationToken);
        var measurement = new Measurement
        {
            WellId = request.WellId,
            RecordedAt = request.RecordedAt.ToUniversalTime(),
            Pressure = request.Pressure,
            Flow = request.Flow
        };
        db.Measurements.Add(measurement);
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(measurement);
    }

    public async Task UpdateAsync(int id, MeasurementRequest request, CancellationToken cancellationToken)
    {
        var measurement = await FindAsync(id, cancellationToken);
        await CheckWellAsync(request.WellId, cancellationToken);
        measurement.WellId = request.WellId;
        measurement.RecordedAt = request.RecordedAt.ToUniversalTime();
        measurement.Pressure = request.Pressure;
        measurement.Flow = request.Flow;
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var measurement = await FindAsync(id, cancellationToken);
        db.Measurements.Remove(measurement);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Measurement> FindAsync(int id, CancellationToken cancellationToken)
    {
        return await db.Measurements.FirstOrDefaultAsync(m => m.Id == id, cancellationToken)
            ?? throw new ServiceException(ServiceError.NotFound, "Измерение не найдено.");
    }

    private async Task CheckWellAsync(int wellId, CancellationToken cancellationToken)
    {
        if (!await db.Wells.AnyAsync(w => w.Id == wellId, cancellationToken))
            throw new ServiceException(ServiceError.Validation, "Указанная скважина не существует.");
    }

    private static MeasurementResponse ToResponse(Measurement measurement)
    {
        return new MeasurementResponse(measurement.Id, measurement.WellId,
            measurement.RecordedAt, measurement.Pressure, measurement.Flow);
    }
}
