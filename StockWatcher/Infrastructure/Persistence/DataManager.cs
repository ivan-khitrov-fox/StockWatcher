using SQLite;
using Microsoft.Maui.Storage;
using StockWatcher.Domain;
using StockWatcher.Domain.Watchers;

namespace StockWatcher.Infrastructure.Persistence;

public class DataManager
{
    private const string _databaseName = "wtch.db3";
    private SQLiteConnection _db;

    public DataManager()
    {
        var dbPath = GetDatabasePath(_databaseName);
        _db = new SQLiteConnection(dbPath,
            openFlags: SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create,
            storeDateTimeAsTicks: true);
        CreateDatabase();
    }

    /// <summary>
    /// Reads all DealWatchers
    /// </summary>
    /// <returns></returns>
    public List<DealWatcher> GetDealWatchers() =>
        _db.Table<DbDealWatcher>().Select(x => x.ToDealWatcher()).ToList();

    /// <summary>
    /// Reads all LimitWatchers
    /// </summary>
    /// <returns></returns>
    public List<LimitWatcher> GetLimitWatchers() =>
        _db.Table<DbLimitWatcher>().Select(x => x.ToLimitWatcher()).ToList();

    /// <summary>
    /// Gets DealWatcher by numeric id
    /// </summary>
    public DealWatcher? GetDealWatcher(int id) =>
        _db.Table<DbDealWatcher>().FirstOrDefault(x => x.Id == id)?.ToDealWatcher();

    /// <summary>
    /// Gets LimitWatcher by numeric id
    /// </summary>
    public LimitWatcher? GetLimitWatcher(int id) =>
        _db.Table<DbLimitWatcher>().FirstOrDefault(x => x.Id == id)?.ToLimitWatcher();

    /// <summary>
    /// Inserts or updates DealWatcher
    /// </summary>
    public void SaveDealWatcher(DealWatcher watcher)
    {
        var db = watcher.ToDbDealWatcher();
        if (db.Id == 0)
            _db.Insert(db);
        else
            _db.Update(db);
    }

    /// <summary>
    /// Inserts or updates LimitWatcher
    /// </summary>
    public void SaveLimitWatcher(LimitWatcher watcher)
    {
        var db = watcher.ToDbLimitWatcher();
        if (db.Id == 0)
            _db.Insert(db);
        else
            _db.Update(db);
    }

    private void CreateDatabase()
    {
        _db.CreateTable<DbDealWatcher>();
        _db.CreateTable<DbLimitWatcher>();
    }

    private string GetDatabasePath(string filename)
    {
        // Use per-app data directory across all platforms (Windows/Android/iOS/etc).
        // This avoids null paths and keeps the database in a writable location.
        return Path.Combine(FileSystem.AppDataDirectory, filename);
    }
}
