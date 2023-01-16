namespace SimpleBacktestLib.Utils;

public static class CandleConverter
{
    /// <summary>
    /// Assumes presorted and a smaller input timeframe than the output timeframe.
    /// </summary>
    /// <param name="inputCandles"></param>
    /// <param name="desiredTimeframe"></param>
    /// <returns></returns>
    public static List<BacktestCandle> ChangeTimeframe(List<BacktestCandle> inputCandles, Timeframe desiredTimeframe)
        => inputCandles
        // Group by floored time based on desiredTimeframe's value (representing seconds)
        .GroupBy(x => new DateTime(x.Time.Ticks - x.Time.Ticks % TimeSpan.TicksPerSecond * (int)desiredTimeframe, x.Time.Kind))
        // Extract the new candle
        .Select(x => new BacktestCandle
        {
            Time = x.Key,
            Open = x.First().Open,
            High = x.Max(y => y.High),
            Low = x.Min(y => y.Low),
            Close = x.Last().Close,
            Volume = x.Sum(y => y.Volume)
        }).ToList();
}
