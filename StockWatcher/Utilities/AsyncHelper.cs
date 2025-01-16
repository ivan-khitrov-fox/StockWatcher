namespace StockWatcher.Utilities;

/// <summary>
/// AsyncHelper. Code sourse: 
/// https://cpratt.co/async-tips-tricks/
/// </summary>
public static class AsyncHelper
{
    private static readonly TaskFactory _taskFactory = new
        TaskFactory(CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskContinuationOptions.None,
                    TaskScheduler.Default);

    /// <summary>
    /// Run async method as sync (for functions).
    /// Example: AsyncHelper.RunSync(() => DoAsyncStuff());
    /// </summary>
    public static TResult RunSync<TResult>(Func<Task<TResult>> func) => _taskFactory
        .StartNew(func)
        .Unwrap()
        .GetAwaiter()
        .GetResult();

    /// <summary>
    /// Run async method as sync (for void methods)
    /// Example: AsyncHelper.RunSync(() => DoAsyncStuff());
    /// </summary>
    public static void RunSync(Func<Task> func) =>_taskFactory
        .StartNew(func)
        .Unwrap()
        .GetAwaiter()
        .GetResult();
}
