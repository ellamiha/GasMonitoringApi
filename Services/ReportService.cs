using GasMonitoringApi.Data;
using GasMonitoringApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GasMonitoringApi.Services;

public class ReportService
{
    private readonly AppDbContext db;

    public ReportService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<List<JournalResponse>> GetJournalAsync(CancellationToken cancellationToken)
    {
        return await db.Measurements.AsNoTracking()
            .OrderByDescending(m => m.RecordedAt).ThenByDescending(m => m.Id)
            .Select(m => new JournalResponse(m.Id, m.Well.Name, m.RecordedAt, m.Pressure, m.Flow))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PressureAlertResponse>> GetPressureAlertsAsync(CancellationToken cancellationToken)
    {
        var measurements = await db.Measurements.AsNoTracking()
            .Include(m => m.Well)
            .Where(m => m.Pressure > m.Well.MaxPressure)
            .OrderByDescending(m => m.RecordedAt).ThenByDescending(m => m.Id)
            .ToListAsync(cancellationToken);

        return measurements.Select(m => new PressureAlertResponse(
            m.Id, m.Well.Name, m.RecordedAt, m.Pressure, m.Well.MaxPressure,
            Math.Round(m.Pressure - m.Well.MaxPressure, 2))).ToList();
    }

    public async Task<List<SummaryResponse>> GetSummaryAsync(CancellationToken cancellationToken)
    {
        var wells = await db.Wells.AsNoTracking()
            .Include(w => w.Measurements.OrderByDescending(m => m.RecordedAt)
                .ThenByDescending(m => m.Id).Take(1))
            .OrderBy(w => w.Id)
            .ToListAsync(cancellationToken);

        return wells.Select(w =>
        {
            var lastMeasurement = w.Measurements.FirstOrDefault();
            return new SummaryResponse(
                w.Id, w.Name, w.PlannedFlow, lastMeasurement?.RecordedAt,
                lastMeasurement?.Flow,
                lastMeasurement == null ? null : w.CalculatePlanPercent(lastMeasurement.Flow),
                lastMeasurement == null ? null : lastMeasurement.IsPressureExceeded(w.MaxPressure));
        }).ToList();
    }
}
