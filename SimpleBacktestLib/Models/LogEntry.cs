namespace SimpleBacktestLib.Models;

public record LogEntry
{
    public string Message { get; private set; }
    public long CandleIndex { get; private set; }
    public LogLevel Level { get; private set; }

    public static LogEntry Create(string message, long candleIndex = -1, LogLevel level = LogLevel.Information)
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
