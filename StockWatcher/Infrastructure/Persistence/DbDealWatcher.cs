using SQLite;

namespace StockWatcher.Infrastructure.Persistence;

[Table("DealWatchers")]
public class DbDealWatcher
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public int Id { get; set; }
    public string Name { get; set; }
    public string SecId { get; set; }
    public int WatcherType { get; set; }
    public DateTime DealTime { get; set; }
    public float StockItemPrice {  get; set; }
    public int StockItemCount { get; set; }
}
