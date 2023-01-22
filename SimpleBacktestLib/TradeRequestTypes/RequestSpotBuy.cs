namespace SimpleBacktestLib.TradeRequestTypes;

public class RequestSpotBuy : TradeRequest
{
    /// <summary>
    /// Defines the input amount for the trade
    /// </summary>
    public TradeInput RequestedInput { get; internal set; } // todo: should be set from default

    internal override void Execute(int candleIndex, BacktestState state)
    {
        decimal price = state.InternalState.CandleData[candleIndex].GetPrice(state.InternalState.CandlePriceTime);
        (decimal trueSpendAmount, bool trueSpendAmountIsFullRequestedAmount) = RequestedInput.GetLiteralValue(state.QuoteBalance);

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
