namespace SimpleBacktestLib;

public record TickData
{
    /// <summary>
    /// Current candle being evaluated
    /// </summary>
    public BacktestCandle CurrentCandle { get; init; }

    /// <summary>
    /// Index of the currently evaluated candle in CandleData
    /// </summary>
    public uint CurrentCandleIndex { get; init; }

    /// <summary>
    /// Full candle dataset being used to evaluate
    /// </summary>
    public ImmutableList<BacktestCandle> HistoricData { get; init; }

    /// <summary>
    /// Current state of the backtest
    /// </summary>
    public BacktestState State { get; init; }
}
