using SQLite;
using StockWatcher.Core.DbModels;
using StockWatcher.Core.Watchers;

namespace StockWatcher.Core.Data;

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
    /// Adds new DealWatcher
    /// </summary>
    /// <param name="watcher"></param>
    public void AddDealWatcher(DealWatcher watcher) => _db.Insert(watcher.ToDbDealWatcher());

    private void CreateDatabase()
    {
        _db.CreateTable<DbDealWatcher>();
        _db.CreateTable<DbLimitWatcher>();
    }

    private string GetDatabasePath(string filename)
    {
#if ANDROID
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), filename);
#elif WINDOWS
        return Path.GetDirectoryName(filename);
#endif
        return null;
    }
}
