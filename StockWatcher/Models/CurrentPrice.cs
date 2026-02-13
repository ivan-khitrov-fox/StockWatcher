namespace StockWatcher.Models;

public class CurrentPrice
{
    public string SecId { get; set; }
    public DateTime Time { get; set; }
    public double Price { get; set; }
}
