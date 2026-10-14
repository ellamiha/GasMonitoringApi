namespace GasMonitoringApi.Dtos;

public record JournalResponse(int MeasurementId, string WellName, DateTimeOffset RecordedAt, double Pressure, double Flow);
