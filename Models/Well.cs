using System.ComponentModel.DataAnnotations;

namespace GasMonitoringApi.Models;

public class Well
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "";

    [Range(0.01, 1000)]
    public double MaxPressure { get; set; }

    [Range(0.01, 1000000)]
    public double PlannedFlow { get; set; }

    public double CalculatePlanPercent(double actualFlow)
    {
        return Math.Round(actualFlow / PlannedFlow * 100, 2);
    }
}