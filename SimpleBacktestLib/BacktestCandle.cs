using SimpleBacktestLib.Internal;

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

    /// <summary>
    /// Gets the price at a point in the candle's lifetime.
    /// </summary>
    /// <param name="atTime"></param>
    /// <returns>Price at the specified time</returns>
    internal decimal GetPrice(PriceTime atTime)
    {
        switch (atTime)
        {
            case PriceTime.AtOpen:
                return Open;
            case PriceTime.AtHigh:
                return High;
            case PriceTime.AtLow:
                return Low;
            case PriceTime.AtClose:
                return Close;
            case PriceTime.AtRandom:
                return new decimal(InternalConstants.RandomGenerator.NextDouble() * (double)(High - Low) + (double)Low);
            default:
                throw new ArgumentException("Invalid PriceTime");
        }
    }
}
