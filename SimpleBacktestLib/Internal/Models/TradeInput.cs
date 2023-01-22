namespace SimpleBacktestLib.Internal.Models;

/// <summary>
/// Input values for a trade
/// </summary>
public record TradeInput
{
    /// <summary>
    /// Create a TradeInput definition to be used for manual trade requests.
    /// </summary>
    /// <param name="amountType">Defines the meaning of the amount parameter</param>
    /// <param name="amount">Amount for the amountType parameter</param>
    /// <param name="allowPartial">
    /// Throws an exception if the balance is insufficient for trade request if false.
    /// Otherwise, it uses whatever remaining balance there is.
    /// </param>
    /// <exception cref="ArgumentException">Throws if amountType/amount combination is invalid</exception>
    public TradeInput(AmountType amountType, decimal amount, bool allowPartial = true)
    {
        // Validate
        if (amountType == AmountType.Percentage && (amount <= 0 || amount > 100))
            throw new ArgumentException("Percent amounts must be between 0 and 100.");
        else if (amountType == AmountType.Absolute && amount <= 0)
            throw new ArgumentException("Absolute amounts must be greater than 0.");

        // Assign
        Type = amountType;
        Amount = amount;
        AllowPartial = allowPartial;
    }

    /// <summary>
    /// Defines what the Amount means.
    /// </summary>
    public AmountType Type { get; private init; }

    /// <summary>
    /// Meaning of this amount depends on the type.
    /// This is not the literal value.
    /// </summary>
    public decimal Amount { get; private init; }

    /// <summary>
    /// True: Throws an exception if the amount is more than we have.
    /// False: Use the balance we do have instead.
    /// </summary>
    public bool AllowPartial { get; private init; }


    /// <summary>
    /// Determine the literal amount for the trade.
    /// </summary>
    /// <param name="availableBalance">Available balance to infer from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If FailOnInsufficientFunds is true, this exception will occur if applicable.</exception>
    public (decimal LiteralValue, bool IsFullAmount) GetLiteralValue(decimal availableBalance)
    {
        // Determine the literal amount based on the type
        decimal literalAmount;
        bool isFullAmount = true;
        switch (Type)
        {
            case AmountType.Max:
                literalAmount = availableBalance;
                break;

            case AmountType.Absolute:
                literalAmount = Amount;
                break;
                
            case AmountType.Percentage:
                literalAmount = availableBalance * (Amount / 100);
                break;

            default:
                throw new NotImplementedException($"AmountType {Type.ToString()} not implemented.");
        }

        // Validate
        if (literalAmount > availableBalance)
        {
            isFullAmount = false;
            if (AllowPartial) literalAmount = availableBalance;
            else throw new ArgumentException("Requested amount is more than available balance.");
        }
        
        // Output
        return (literalAmount, isFullAmount);
    }
}
