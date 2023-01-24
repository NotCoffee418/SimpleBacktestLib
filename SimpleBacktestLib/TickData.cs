using SimpleBacktestLib.TradingManagers;

namespace SimpleBacktestLib;

/// <summary>
/// Data passed down to the Tick() function for understanding and manipulating the current state of the backtest.
/// </summary>
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
    /// Access the current state of the backtest.
    /// </summary>
    public BacktestState State { get; init; }

    /// <summary>
    /// Execute simulated trades for your backtest
    /// </summary>
    public CommonTradeManager Trade { get => State.TradeManagerInstance; }
}
