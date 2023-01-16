namespace SimpleBacktestLib.Internal.Models;

/// <summary>
/// Internal-only setup properties.
/// Defined by builder and accessed by the engine.
/// </summary>
internal class SetupDefinitions
{
    /// <summary>
    /// First index of CandleData that should be evaluated
    /// </summary>
    internal int EvaluateLastIndex { get; set; } = -1;

    /// <summary>
    /// Last index of CandleData that should be evaluated
    /// </summary>
    internal int EvaluateFirstIndex { get; set; } = -1;

    /// <summary>
    /// Full candle data, including unevaluated data.
    /// </summary>
    internal ImmutableList<BacktestCandle> CandleData { get; set; }

    /// <summary>
    /// Tick functions that should be called, in order.
    /// </summary>
    internal List<Func<TickData, IEnumerable<TradeRequest>?>> OnTickFunctions { get; } = new();
    
    /// <summary>
    /// Post-tick functions that should be called, in order.
    /// </summary>
    internal List<Action<(TickData TickData, IEnumerable<TradeRequest> ExecutedTrades)>> PostTickFunctions { get; } = new();
}
