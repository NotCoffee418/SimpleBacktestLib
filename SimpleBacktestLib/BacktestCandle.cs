namespace SimpleBacktestLib;

/// <summary>
/// OHLCV model for SimpleBacktestLib.
/// You should write your own implementation to convert any OHLCV for this library.
/// Candles should be loaded in chronological order as SimpleBacktestLib will not sort them.
/// </summary>
public record BacktestCandle
{
    public DateTime Time { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public decimal Volume { get; init; }
}
