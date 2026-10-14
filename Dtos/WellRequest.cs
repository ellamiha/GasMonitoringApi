using System.ComponentModel.DataAnnotations;

namespace GasMonitoringApi.Dtos;

public class WellRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "";

    [Range(0.01, 1000)]
    public double MaxPressure { get; set; }

    [Range(0.01, 1000000)]
    public double PlannedFlow { get; set; }
}
