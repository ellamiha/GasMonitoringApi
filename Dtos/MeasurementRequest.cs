using System.ComponentModel.DataAnnotations;

namespace GasMonitoringApi.Dtos;

public class MeasurementRequest
{
    [Range(1, int.MaxValue)]
    public int WellId { get; set; }

    public DateTimeOffset RecordedAt { get; set; } = DateTimeOffset.UtcNow;

    [Range(0, 1000)]
    public double Pressure { get; set; }

    [Range(0, 1000000)]
    public double Flow { get; set; }
}
