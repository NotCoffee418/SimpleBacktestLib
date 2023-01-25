namespace SimpleBacktestLib.Models;

public record Fee
{
    public Fee(AmountType amountType, decimal amount, FeeSource feeSource)
    {
        // Validate
        if (amountType == AmountType.Percentage && (amount < 0 || amount > 100))
            throw new ArgumentException("Percent amounts must be between 0 and 100.");
        else if (amountType == AmountType.Absolute && amount < 0)
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
}
