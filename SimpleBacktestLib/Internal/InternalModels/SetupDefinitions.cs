namespace SimpleBacktestLib.Internal.InternalModels;

/// <summary>
/// Internal-only setup properties.
/// Defined by builder and accessed by the engine.
/// </summary>
internal class SetupDefinitions
{
    /// <summary>
    /// Available base budget. 
    /// This does not track any changes and should only be set in BacktestBuilder.
    /// </summary>
    internal decimal StartingBaseBalance { get; set; } = 0;

    /// <summary>
    /// Available quote budget.
    /// This does not track any changes and should only be set in BacktestBuilder.
    /// </summary>
    internal decimal StartingQuoteBalance { get; set; } = 10000;

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
    /// Fees applied to spot trades, in order.
    /// </summary>
    internal List<Fee> SpotFees { get; set; } = new() {
        new(AmountType.Percentage, 0.1m, FeeSource.Base)
    };

    /// <summary>
    /// First index of CandleData that should be evaluated.
    /// Default value defined by CreateBuilder().
    /// </summary>
    internal int EvaluateLastIndex { get; set; }

    /// <summary>
    /// Last index of CandleData that should be evaluated.
    /// Default value defined by CreateBuilder().
    /// </summary>
    internal int EvaluateFirstIndex { get; set; }

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
