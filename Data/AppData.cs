using GasMonitoringApi.Models;

namespace GasMonitoringApi.Data;

public class AppData
{
    public object SyncRoot { get; } = new();

    public List<Well> Wells { get; } = new()
    {
        new Well
        {
            Id = 1,
            Name = "Скважина 1",
            MaxPressure = 12,
            PlannedFlow = 100
        },
        new Well
        {
            Id = 2,
            Name = "Скважина 2",
            MaxPressure = 10,
            PlannedFlow = 80
        }
    };

    public List<Measurement> Measurements { get; } = new()
    {
        new Measurement
        {
            Id = 1,
            WellId = 1,
            RecordedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            Pressure = 11,
            Flow = 90
        },
        new Measurement
        {
            Id = 2,
            WellId = 2,
            RecordedAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            Pressure = 13,
            Flow = 60
        }
    };

    private int nextWellId = 3;
    private int nextMeasurementId = 3;

    public int GetNextWellId()
    {
        return nextWellId++;
    }

    public int GetNextMeasurementId()
    {
        return nextMeasurementId++;
    }
}