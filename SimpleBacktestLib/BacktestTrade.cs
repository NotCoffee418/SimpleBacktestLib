namespace SimpleBacktestLib;

public record BacktestTrade
{
    public BacktestTrade(
        TradeOperation action,
        decimal quotePrice,
        decimal baseAmount,
        decimal quoteAmount,
        long candleIndex,
        DateTime candleTime)
    {
        Action = action;
        QuotePrice = quotePrice;
        BaseAmount = baseAmount;
        QuoteAmount = quoteAmount;
        CandleIndex = candleIndex;
        CandleTime = candleTime;
    }

    /// <summary>
    /// Trade action taken
    /// </summary>
    public TradeOperation Action { get; private init; }

    /// <summary>
    /// Trade price
    /// </summary>
    public decimal QuotePrice { get; private init; }

    /// <summary>
    /// Amount of base asset that was exchanged
    /// </summary>
    public decimal BaseAmount { get; private init; }

    /// <summary>
    /// Amount of quote asset that was exchanged
    /// </summary>
    public decimal QuoteAmount { get; private init; }

    /// <summary>
    /// Index of the candle in the backtest data
    /// </summary>
    public long CandleIndex { get; private init; }

    /// <summary>
    /// Timestamp of the candle
    /// </summary>
    public DateTime CandleTime { get; private init; }
}
