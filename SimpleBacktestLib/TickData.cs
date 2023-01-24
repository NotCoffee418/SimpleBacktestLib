using SimpleBacktestLib.Internal.Helpers;
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

    /// <summary>
    /// Add a log entry and trigger OnLogEntry.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="level"></param>
    public void AddLogEntry(string message, LogLevel level = LogLevel.Information)
        => LogHandler.AddLogEntry(State, message, CurrentCandleIndex, level);
}
