namespace SimpleBacktestLib.Internal.Logic;

internal static class ValueAssessment
{
    /// <summary>
    /// Get the combined value of both assets in the defined asset type.
    /// Example: 
    /// baseAmount: 0.6, quoteAmount: 600, price: 1000.
    /// Base value: 1.2
    /// Quote value: 1200
    /// </summary>
    /// <param name="asset"></param>
    /// <param name="baseAmount"></param>
    /// <param name="quoteAmount"></param>
    /// <param name="quotePrice"></param>
    /// <returns></returns>
    internal static decimal GetCombinedValue(AssetType asset, decimal baseAmount, decimal quoteAmount, decimal quotePrice)
        => asset == AssetType.Base ?
            baseAmount + (quoteAmount / quotePrice) :
            quoteAmount + (baseAmount * quotePrice);


    /// <summary>
    /// Shortcut function to calculate base value from quote value and price.
    /// </summary>
    /// <param name="quoteAmount"></param>
    /// <param name="quotePrice"></param>
    /// <returns></returns>
    internal static decimal CalcBase(decimal quoteAmount, decimal quotePrice)
        => quoteAmount / quotePrice;

    /// <summary>
    /// Shortcut function to calculate base value from quote value and price
    /// </summary>
    /// <param name="baseAmount"></param>
    /// <param name="quotePrice"></param>
    /// <returns></returns>
    internal static decimal CalcQuote(decimal baseAmount, decimal quotePrice)
        => baseAmount * quotePrice;

    /// <summary>
    /// Calculate the input amount from user settings.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amountRequested"></param>
    /// <param name="availableBalance"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    internal static (bool IsFullRequestedAmount, decimal TrueAmount) GetSpendAmount(AmountType type, decimal amountRequested, decimal availableBalance)
    {
        switch (type)
        {
            case AmountType.Max:
                return (true, availableBalance);
                
            case AmountType.Fixed:
                bool isFullRequestedAmount = amountRequested <= availableBalance;
                return (isFullRequestedAmount, isFullRequestedAmount ? amountRequested : availableBalance);
                
            case AmountType.Percentage:
                decimal ratio = amountRequested / 100;
                if (ratio > 1 || ratio < 0)
                    throw new ArgumentException("Percentage must be between 0 and 100.");
                return (true, availableBalance * ratio); ;
                
            default:
                throw new NotImplementedException($"AmountType {type.ToString()} not implemented.");
        }
    }
}
