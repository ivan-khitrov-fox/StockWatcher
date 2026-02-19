using SQLite;

namespace StockWatcher.Infrastructure.Persistence;

[Table("VolatilitySpikeWatchers")]
public class DbVolatilitySpikeWatcher
{
    [PrimaryKey, AutoIncrement, Column("id")]
    public int Id { get; set; }

    public string Name { get; set; }
    public string SecId { get; set; }
    public int WatcherType { get; set; }

    public float PercentThreshold { get; set; }
    public int WindowHours { get; set; }
}
