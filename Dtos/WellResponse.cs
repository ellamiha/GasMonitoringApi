namespace GasMonitoringApi.Dtos;

public record WellResponse(int Id, string Name, double MaxPressure, double PlannedFlow);
