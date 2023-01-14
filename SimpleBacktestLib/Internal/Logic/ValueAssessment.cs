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
}
