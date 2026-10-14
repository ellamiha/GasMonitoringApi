namespace GasMonitoringApi.Models;

public class Well
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double MaxPressure { get; set; }
    public double PlannedFlow { get; set; }
    public List<Measurement> Measurements { get; set; } = new();

    public double CalculatePlanPercent(double actualFlow)
    {
        return Math.Round(actualFlow / PlannedFlow * 100, 2);
    }
}
