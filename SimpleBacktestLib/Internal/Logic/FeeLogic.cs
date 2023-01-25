namespace SimpleBacktestLib.Internal.Logic;

internal static class FeeLogic
{
    /// <summary>
    /// Calculate the total fees for a trade from a list of Fees.
    /// </summary>
    /// <param name="fees"></param>
    /// <param name="direction"></param>
    /// <param name="inputAmount"></param>
    /// <param name="quotePrice"></param>
    /// <returns></returns>
    public static (decimal BaseFee, decimal QuoteFee) CalculateCombinedFees(IEnumerable<Fee> fees, TradeType direction, decimal inputAmount, decimal quotePrice)
    {
        decimal combinedBaseFees = 0, combinedQuoteFees = 0, iterBaseFee, iterQuoteFee;
        foreach (Fee fee in fees)
        {
            (iterBaseFee, iterQuoteFee) = CalculateFee(fee, direction, inputAmount, quotePrice);
            combinedBaseFees += iterBaseFee;
            combinedQuoteFees += iterQuoteFee;
        }
        return (combinedBaseFees, combinedQuoteFees);
    }
    
    /// <summary>
    /// Calculates the literal fee value for both base and quote.
    /// Fees can be taken from one or both assets.
    /// </summary>
    /// <param name="fee"></param>
    /// <param name="direction"></param>
    /// <param name="inputAmount"></param>
    /// <param name="quotePrice"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static (decimal BaseFee, decimal QuoteFee) CalculateFee(Fee fee, TradeType direction, decimal inputAmount, decimal quotePrice)
    {
        // Filter Source down to Base, Quote or Both
        (FeeSource actualFeeSource, TradeOperation operation) = GetContextualFeeSourceAndOperation(fee.Source, direction);
        decimal inputAmountAsBase = operation == TradeOperation.Buy ? ValueAssessment.CalcBase(inputAmount, quotePrice) : inputAmount;
        decimal inputAmountAsQuote = operation == TradeOperation.Buy ? inputAmount : ValueAssessment.CalcQuote(inputAmount, quotePrice);

        // Calculate modifier per asset as ratio
        decimal baseFeeModifier, quoteFeeModifier;
        switch (actualFeeSource)
        {
            case FeeSource.Base:
                baseFeeModifier = 1;
                quoteFeeModifier = 0;
                break;

            case FeeSource.Quote:
                baseFeeModifier = 0;
                quoteFeeModifier = 1;
                break;
                 
            case FeeSource.Both:
                baseFeeModifier = 0.5m;
                quoteFeeModifier = 0.5m;
                break;

            default:
                throw new ArgumentOutOfRangeException("Invalid FeeSource");
        }

        // Calculate the actual fee
        decimal baseFee, quoteFee;
        switch (fee.Type)
        {
            case AmountType.Absolute:
                baseFee = fee.Amount * baseFeeModifier;
                quoteFee = fee.Amount * quoteFeeModifier;
                break;

            case AmountType.Percentage:
                baseFee = inputAmountAsBase * (fee.Amount / 100) * baseFeeModifier;
                quoteFee = inputAmountAsQuote * (fee.Amount / 100) * quoteFeeModifier;
                break;

            default:
                throw new ArgumentOutOfRangeException("Invalid AmountType");
        }

        // Return result
        return (baseFee, quoteFee);
    }

    /// <summary>
    /// Change the fee source if it's a contextual value
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private static (FeeSource FeeSource, TradeOperation Operation) GetContextualFeeSourceAndOperation(FeeSource inputFeeSource, TradeType direction)
    {
        // Decide the operation
        TradeOperation operation = TradeOperation.UnspecifiedOrNotApplicable;
        if (direction == TradeType.SpotBuy || direction == TradeType.MarginLong)
            operation = TradeOperation.Buy;
        else if (direction == TradeType.SpotSell || direction == TradeType.MarginShort)
            operation = TradeOperation.Sell;

        // Return if not a contextual value
        if (inputFeeSource != FeeSource.Input && inputFeeSource != FeeSource.Output)
            return (inputFeeSource, operation);

        // Determine the fee source based on the direction
        if (operation == TradeOperation.Buy)
            return (inputFeeSource == FeeSource.Input ? FeeSource.Quote : FeeSource.Base, operation);
        else if (operation == TradeOperation.Sell)
            return (inputFeeSource == FeeSource.Input ? FeeSource.Base : FeeSource.Quote, operation);

        // We should not still be here
        throw new InvalidOperationException($"Failed to infer contextual FeeSource {inputFeeSource} from TradeType {direction}");
    }

    
}
