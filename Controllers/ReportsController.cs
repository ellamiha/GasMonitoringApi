
using GasMonitoringApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasMonitoringApi.Controllers;
[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly AppData data;

    public ReportsController(AppData data)
    {
        this.data = data;
    }

    [HttpGet("journal")]
    public IActionResult GetJournal()
    {
        lock (data.SyncRoot)
        {
            var result = data.Measurements
                .Join(
                    data.Wells,
                    m => m.WellId,
                    w => w.Id,
                    (m, w) => new
                    {
                        MeasurementId = m.Id,
                        WellName = w.Name,
                        m.RecordedAt,
                        m.Pressure,
                        m.Flow
                    })
                .OrderByDescending(x => x.RecordedAt)
                .ToList();

            return Ok(result);
        }
    }

    [HttpGet("pressure-alerts")]
    public IActionResult GetPressureAlerts()
    {
        lock (data.SyncRoot)
        {
            var result = data.Measurements
                .Join(
                    data.Wells,
                    m => m.WellId,
                    w => w.Id,
                    (m, w) => new
                    {
                        Measurement = m,
                        Well = w
                    })
                .Where(x => x.Measurement.IsPressureExceeded(
                    x.Well.MaxPressure))
                .Select(x => new
                {
                    MeasurementId = x.Measurement.Id,
                    WellName = x.Well.Name,
                    x.Measurement.RecordedAt,
                    x.Measurement.Pressure,
                    x.Well.MaxPressure,
                    Excess = Math.Round(
                        x.Measurement.Pressure - x.Well.MaxPressure,
                        2)
                })
                .ToList();

            return Ok(result);
        }
    }

    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        lock (data.SyncRoot)
        {
            var result = data.Wells
                .GroupJoin(
                    data.Measurements,
                    w => w.Id,
                    m => m.WellId,
                    (w, measurements) => new
                    {
                        Well = w,
                        LastMeasurement = measurements
                            .OrderByDescending(m => m.RecordedAt)
                            .ThenByDescending(m => m.Id)
                            .FirstOrDefault()
                    })
                .Select(x => new
                {
                    WellId = x.Well.Id,
                    WellName = x.Well.Name,
                    x.Well.PlannedFlow,
                    RecordedAt = x.LastMeasurement?.RecordedAt,
                    ActualFlow = x.LastMeasurement?.Flow,
                    PlanPercent = x.LastMeasurement == null
                        ? (double?)null
                        : x.Well.CalculatePlanPercent(
                            x.LastMeasurement.Flow),
                    PressureExceeded = x.LastMeasurement == null
                        ? (bool?)null
                        : x.LastMeasurement.IsPressureExceeded(
                            x.Well.MaxPressure)
                })
                .ToList();

            return Ok(result);
        }
    }
}