namespace GasMonitoringApi.Models;

public class Measurement
{
    public int Id { get; set; }
    public int WellId { get; set; }
    public Well Well { get; set; } = null!;
    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;
    public double Pressure { get; set; }
    public double Flow { get; set; }

    public bool IsPressureExceeded(double maxPressure)
    {
        return Pressure > maxPressure;
    }
}
