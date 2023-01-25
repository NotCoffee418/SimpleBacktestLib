namespace SimpleBacktestLib.Models;

public record LogEntry
{
    /// <summary>
    /// Log message without status or candle indicators
    /// </summary>
    public string Message { get; private set; }
    
    /// <summary>
    /// Index of the candle in the backtest data
    /// </summary>
    public long CandleIndex { get; private set; }

    /// <summary>
    /// Loglevel of the entry
    /// </summary>
    public LogLevel Level { get; private set; }
    
    internal static LogEntry Create(string message, long candleIndex = -1, LogLevel level = LogLevel.Information)
        => new()
        {
            Message = message,
            CandleIndex = candleIndex,
            Level = level,
        };

    public override string ToString()
        => $"[{ShortenedLevelString()}][{CandleIndex}] {Message}";

    private string ShortenedLevelString()
        => Level switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRIT",
            _ => "???",
        };
}
