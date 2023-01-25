namespace SimpleBacktestLib.Internal.Helpers;

internal static class LogHandler
{
    /// <summary>
    /// Run all actions required when adding a log entry.
    /// Can be called by internal anywhere, or by user through TickData.
    /// Calling this will fire OnLogEntry() functions
    /// </summary>
    /// <param name="state">State to append the log to</param>
    /// <param name="entryText"></param>
    /// <param name="candleIndex"></param>
    /// <param name="level"></param>
    public static void AddLogEntry(BacktestState state, string entryText, long candleIndex = -1, LogLevel level = LogLevel.Information)
    {
        var entry = LogEntry.Create(entryText, candleIndex, level);
        state.LogEntries.Add(entry);
        state.SetupConfig.OnLogEntryFunctions.ForEach(f => f(entry, state));
    }
}
