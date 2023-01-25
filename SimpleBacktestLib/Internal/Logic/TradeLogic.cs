namespace SimpleBacktestLib.Internal.Logic;

/// <summary>
/// Logic for general swap-like trades such as spot
/// </summary>
internal static class TradeLogic
{
    /// <summary>
    /// Calculate the assets gained and removed from buying after fees.
    /// </summary>
    /// <param name="quotePrice"></param>
    /// <param name="input"></param>
    /// <param name="fees"></param>
    /// <returns>Positive values changed amounts</returns>
    internal static (decimal BaseGained, decimal QuoteRemoved, bool IsFullRequestedValueUsed) 
        SimulateBuy(decimal quotePrice, TradeInput input, List<Fee> fees)
    {
        // Calculate fees
        (decimal literalInputBeforeFees, bool isFullValue) = input.GetLiteralValue(quotePrice);
        (decimal literalBaseFees, decimal literalQuoteFees) = FeeLogic.CalculateCombinedFees(
            fees, TradeType.SpotBuy, literalInputBeforeFees, quotePrice);
        decimal inputAmountAfterQuoteFee = literalInputBeforeFees - literalQuoteFees;

        // Calculate output amount
        decimal literalOutputAfterFees = ValueAssessment.CalcTradeOutput(
            TradeOperation.Buy,
            inputAmountAfterQuoteFee,
            quotePrice,
            literalBaseFees,
            0); // Already applied

        return (literalOutputAfterFees, literalInputBeforeFees, isFullValue);
    }

    /// <summary>
    /// Calculate the assets gained and removed from selling after fees.
    /// </summary>
    /// <param name="quotePrice"></param>
    /// <param name="input"></param>
    /// <param name="fees"></param>
    /// <returns>Positive values changed amounts</returns>
    internal static (decimal BaseRemoved, decimal QuoteGained, bool IsFullRequestedValueUsed) SimulateSell(decimal quotePrice, TradeInput input, List<Fee> fees)
    {
        // Calculate fees
        (decimal literalInputBeforeFees, bool isFullValue) = input.GetLiteralValue(quotePrice);
        (decimal literalBaseFees, decimal literalQuoteFees) = FeeLogic.CalculateCombinedFees(
            fees, TradeType.SpotSell, literalInputBeforeFees, quotePrice);
        decimal inputAmountAfterBaseFee = literalInputBeforeFees - literalBaseFees;

        // Calculate output amount
        decimal literalOutputAfterFees = ValueAssessment.CalcTradeOutput(
            TradeOperation.Sell,
            inputAmountAfterBaseFee,
            quotePrice,
            0, // Already applied
            literalQuoteFees); 

        return (literalInputBeforeFees, literalOutputAfterFees, isFullValue);
    }
}
