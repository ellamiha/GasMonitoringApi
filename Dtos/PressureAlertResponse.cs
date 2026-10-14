namespace GasMonitoringApi.Dtos;

public record PressureAlertResponse(int MeasurementId, string WellName, DateTimeOffset RecordedAt, double Pressure, double MaxPressure, double Excess);
