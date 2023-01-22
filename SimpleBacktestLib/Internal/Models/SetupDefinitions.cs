namespace SimpleBacktestLib.Internal.Models;

/// <summary>
/// Internal-only setup properties.
/// Defined by builder and accessed by the engine.
/// </summary>
internal class SetupDefinitions
{
    internal decimal QuoteBudget { get; set; } = 10000;
    internal decimal BaseBudget { get; set; } = 0;

    /// <summary>
    /// Specify the default quote input size for spot trading.
    /// </summary>
    internal TradeInput DefaultSpotBuyOrderSize { get; set; } = new(AmountType.Max, 0);

    /// <summary>
    /// Specify the default base input size for spot trading.
    /// </summary>
    internal TradeInput DefaultSpotSellOrderSize { get; set; } = new(AmountType.Max, 0);

    /// <summary>
    /// Specify the default quote input size for margin trading.
    internal TradeInput DefaultMarginLongOrderSize { get; set; } = new(AmountType.Max, 0);

    /// <summary>
    /// Specify the default base input size for margin trading.
    /// </summary>
    internal TradeInput DefaultMarginShortOrderSize { get; set; } = new(AmountType.Max, 0);

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

    /// <summary>
    /// Specifies the price to use on each candle.
    /// </summary>
    internal PriceTime CandlePriceTime { get; set; } = PriceTime.AtOpen;
}
