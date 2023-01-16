namespace SimpleBacktestLib.Internal.Models;

/// <summary>
/// Internal-only setup properties.
/// Defined by builder and accessed by the engine.
/// </summary>
internal class SetupDefinitions
{
    internal IEnumerable<BacktestCandle> CandleData { get; set; }

    internal List<Func<TickData, IEnumerable<TradeRequest>?>> OnTickFunctions { get; } = new();
    
    internal List<Action<(TickData TickData, IEnumerable<TradeRequest> ExecutedTrades)>> PostTickFunctions { get; } = new();
}
