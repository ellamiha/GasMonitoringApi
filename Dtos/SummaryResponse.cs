namespace GasMonitoringApi.Dtos;

public record SummaryResponse(int WellId, string WellName, double PlannedFlow, DateTimeOffset? RecordedAt, double? ActualFlow, double? PlanPercent, bool? PressureExceeded);
