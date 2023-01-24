using SimpleBacktestLib.Models;
using SimpleBacktestLib.TradingManagers;

namespace SimpleBacktestLib.Internal.Models;

/// <summary>
/// Internal-only setup properties.
/// Defined by builder and accessed by the engine.
/// </summary>
internal class SetupDefinitions
{
    /// <summary>
    /// Available quote budget. Modified internally through trades
    /// </summary>
    internal decimal QuoteBudget { get; set; } = 10000;

    /// <summary>
    /// Available base budget. Modified internally through trades.
    /// </summary>
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
    internal List<Action<BacktestState>> OnTickFunctions { get; } = new();

    /// <summary>
    /// Function that triggers when a log entry is made.
    /// </summary>
    internal List<Action<LogEntry, BacktestState>> OnLogEntryFunctions { get; } = new();
    
    /// <summary>
    /// Post-tick functions that should be called, in order.
    /// </summary>
    internal List<Action<BacktestState>> PostTickFunctions { get; } = new();

    /// <summary>
    /// Specifies the price to use on each candle.
    /// </summary>
    internal PriceTime CandlePriceTime { get; set; } = PriceTime.AtOpen;
    
    /// <summary>
    /// Leverage for margin loans.
    /// </summary>
    internal decimal MarginLeverageRatio { get; set; } = 5;

    /// <summary>
    /// Ratio below which the margin position will liquidate
    /// </summary>
    internal decimal MarginLiquidationRatio { get; set; } = 0.1m;
}
