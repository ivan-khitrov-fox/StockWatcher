using SQLite;

namespace StockWatcher.Core.DbModels;

[Table("LimitWatchers")]
public class DbLimitWatcher
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public int Id { get; set; }
    public string Name { get; set; }
    public string SecId { get; set; }
    public int WatcherType { get; set; }
    public DateTime DealTime { get; set; }
    public float? WarningLevel { get; set; }
    public float? AlarmLevel { get; set; }
    public float? StartPrice { get; set; }
    public float? WarningPct { get; set; }
    public float? AlarmPct { get; set; }
}
