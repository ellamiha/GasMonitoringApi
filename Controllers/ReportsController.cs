using GasMonitoringApi.Dtos;
using GasMonitoringApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasMonitoringApi.Controllers;

[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService service;

    public ReportsController(ReportService service)
    {
        this.service = service;
    }

    [HttpGet("journal")]
    public async Task<ActionResult<List<JournalResponse>>> GetJournal(CancellationToken cancellationToken)
    {
        return Ok(await service.GetJournalAsync(cancellationToken));
    }

    [HttpGet("pressure-alerts")]
    public async Task<ActionResult<List<PressureAlertResponse>>> GetPressureAlerts(CancellationToken cancellationToken)
    {
        return Ok(await service.GetPressureAlertsAsync(cancellationToken));
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<SummaryResponse>>> GetSummary(CancellationToken cancellationToken)
    {
        return Ok(await service.GetSummaryAsync(cancellationToken));
    }
}
