namespace SimpleBacktestLib.Internal.Models;

public record Fee
{
    public Fee(AmountType amountType, decimal amount, FeeSource feeSource)
    {
        // Validate
        if (amountType == AmountType.Percentage && (amount <= 0 || amount > 100))
            throw new ArgumentException("Percent amounts must be between 0 and 100.");
        else if (amountType == AmountType.Absolute && amount <= 0)
            throw new ArgumentException("Absolute amounts must be greater than 0.");
        else if (amountType == AmountType.Max)
            throw new ArgumentException("Max amounts are not allowed for fees as it would consume the whole balance as a fee.");

        // Assign
        Type = amountType;
        Amount = amount;
        Source = feeSource;
    }

    public AmountType Type { get; }
    public decimal Amount { get; }
    public FeeSource Source { get; }

    public (decimal BaseFee, decimal QuoteFee) CalculateFee(TradeType direction, decimal inputAmount, decimal quotePrice)
    {
        // Filter Source down to Base, Quote or Both
        (FeeSource actualFeeSource, TradeOperation operation) = GetContextualFeeSourceAndOperation(direction);
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
        switch (Type)
        {
            case AmountType.Absolute:
                baseFee = Amount * baseFeeModifier;
                quoteFee = Amount * quoteFeeModifier;
                break;

            case AmountType.Percentage:
                baseFee = inputAmountAsBase * (Amount / 100) * baseFeeModifier;
                quoteFee = inputAmountAsQuote * (Amount / 100) * quoteFeeModifier;
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
    private (FeeSource FeeSource, TradeOperation Operation) GetContextualFeeSourceAndOperation(TradeType direction)
    {
        // Decide the operation
        TradeOperation operation = TradeOperation.UnspecifiedOrNotApplicable;
        if (direction == TradeType.SpotBuy || direction == TradeType.MarginLong)
            operation = TradeOperation.Buy;
        else if (direction == TradeType.SpotSell || direction == TradeType.MarginShort)
            operation = TradeOperation.Sell;

        // Return if not a contextual value
        if (Source != FeeSource.Input && Source != FeeSource.Output)
            return (Source, operation);

        // Determine the fee source based on the direction
        if (operation == TradeOperation.Buy)
            return (Source == FeeSource.Input ? FeeSource.Quote : FeeSource.Base, operation);
        else if (operation == TradeOperation.Sell)
            return (Source == FeeSource.Input ? FeeSource.Base : FeeSource.Quote, operation);

        // We should not still be here
        throw new InvalidOperationException($"Failed to infer contextual FeeSource {Source} from TradeType {direction}");
    }
}
