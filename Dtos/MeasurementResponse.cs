namespace GasMonitoringApi.Dtos;

public record MeasurementResponse(int Id, int WellId, DateTimeOffset RecordedAt, double Pressure, double Flow);
