using StockWatcher.Core.DbModels;
using StockWatcher.Core.Watchers;
using StockWatcher.Enums;

namespace StockWatcher.Core.Data;

public static class DataConverters
{
    public static DealWatcher ToDealWatcher(this DbDealWatcher model) => new DealWatcher
    {
        Id = "D" + model.Id,
        Name = model.Name,
        DealTime = model.DealTime,
        Type = (WatcherType) model.WatcherType,
        SecId = model.SecId,
        StockItemCount = model.StockItemCount,
        StockItemPrice = Convert.ToDouble(model.StockItemPrice)
    };

    public static DbDealWatcher ToDbDealWatcher(this DealWatcher model) => new DbDealWatcher
    {
        Id = Convert.ToInt32(model.Id.Substring(1)),
        Name = model.Name,
        DealTime = model.DealTime,
        WatcherType = (int)model.Type,
        SecId = model.SecId,
        StockItemCount = model.StockItemCount,
        StockItemPrice = Convert.ToSingle(model.StockItemPrice)
    };

    public static LimitWatcher ToLimitWatcher(this DbLimitWatcher model) => new LimitWatcher
    {
        Id = "L" + model.Id,
        Name = model.Name,
        DealTime = model.DealTime,
        Type = (WatcherType)model.WatcherType,
        SecId = model.SecId,
        AlarmLevel = model.AlarmLevel == null ? null : Convert.ToDouble(model.AlarmLevel),
        WarningLevel = model.WarningLevel == null ? null : Convert.ToDouble(model.WarningLevel),
        StartPrice = model.StartPrice == null ? null : Convert.ToDouble(model.StartPrice),
        WarningPct = model.WarningPct,
        AlarmPct = model.AlarmPct,
    };

    public static DbLimitWatcher ToDbLimitWatcher(this LimitWatcher model) => new DbLimitWatcher
    {
        Id = Convert.ToInt32(model.Id.Substring(1)),
        Name = model.Name,
        DealTime = model.DealTime,
        WatcherType = (int)model.Type,
        SecId = model.SecId,
        AlarmLevel = model.AlarmLevel == null ? null : Convert.ToSingle(model.AlarmLevel),
        WarningLevel = model.WarningLevel == null ? null : Convert.ToSingle(model.WarningLevel),
        StartPrice = model.StartPrice == null ? null : Convert.ToSingle(model.StartPrice),
        WarningPct = model.WarningPct,
        AlarmPct = model.AlarmPct,
    };
}
