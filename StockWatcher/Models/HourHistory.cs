namespace StockWatcher.Models;

public class HourHistory
{
    public string SecId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double StartPrice { get; set; }
    public double EndPrice { get; set; }
}
