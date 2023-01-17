namespace SimpleBacktestLib.TradeRequestTypes;

public class RequestSpotBuy : TradeRequest
{
    /// <summary>
    /// Amount to trade. Meaning changed by relative to AmountTypeOverride.
    /// Falls back to backtest settings if null.
    /// </summary>
    public decimal? AmountRequestOverride { get; internal set; } = null;

    /// <summary>
    /// Amount type. Define meaning for AmountRequestOverride.
    /// Falls back to backtest settings if null.
    /// </summary>
    public AmountType? AmountTypeOverride { get; internal set; } = null;

    internal override void Execute(int candleIndex, BacktestState state)
    {
        decimal price = state.InternalState.CandleData[candleIndex].GetPrice(state.InternalState.CandlePriceTime);
        (bool trueSpendAmountIsFullRequestedAmount, decimal trueSpendAmount) = ValueAssessment.GetSpendAmount(
            AmountTypeOverride ?? state.InternalState.DefaultQuoteAmountType,
            AmountRequestOverride ?? state.InternalState.DefaultQuoteAmountRequest,
            state.QuoteBalance);

        // Validate price and amount
        if (price <= 0)
        {
            LogEntry = $"Candle {candleIndex} is invalid. Skipping request on this candle.";
            Status = TradeStatus.Cancelled;
            return;
        }
        if (trueSpendAmount <= 0)
        {
            LogEntry = "Buy requested but insufficient quote balance was available.";
            Status = TradeStatus.Cancelled;
            return;
        }




        

        throw new NotImplementedException();
    }
}
