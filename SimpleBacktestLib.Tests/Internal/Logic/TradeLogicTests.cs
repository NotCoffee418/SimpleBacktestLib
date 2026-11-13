namespace SimpleBacktestLib.Tests.Internal.Logic;

public class TradeLogicTests
{
    public static TheoryData<decimal, decimal, decimal, decimal, decimal> BuyCases => new()
    {
        { 1000m, 100m, 0m, 1m, 0.099m },
        { 1000m, 100m, 1m, 0m, 0.099m },
        { 1000m, 100m, 1m, 1m, 0.098m },
        { 1000m, 100m, 0m, 0m, 0.1m },
    };

    [Theory]
    [MemberData(nameof(BuyCases))]
    public void SimulateBuy_SingleQuoteFee(
        decimal quotePrice,
        decimal inputAmount,
        decimal pctBaseFee,
        decimal pctQuoteFee,
        decimal expectedBaseGained)
    {
        TradeInput tradeInput = new TradeInput(AmountType.Absolute, inputAmount, false);
        Fee baseFee = new Fee(AmountType.Percentage, pctBaseFee, FeeSource.Base);
        Fee quoteFee = new Fee(AmountType.Percentage, pctQuoteFee, FeeSource.Quote);
        (decimal baseGained, decimal quoteRemoved, bool fullInputUsed) = TradeLogic.SimulateBuy(
            quotePrice, inputAmount, tradeInput, new() { baseFee, quoteFee });

        Assert.True(fullInputUsed);
        Assert.Equal(inputAmount, quoteRemoved); // Full input should be consumed
        Assert.Equal(expectedBaseGained, baseGained);
    }

    public static TheoryData<decimal, decimal, decimal, decimal, decimal> SellCases => new()
    {
        { 1000m, 0.1m, 0m, 1m, 99m },
        { 1000m, 0.1m, 1m, 0m, 99m },
        { 1000m, 0.1m, 1m, 1m, 98m },
        { 1000m, 0.1m, 0m, 0m, 100m },
    };

    [Theory]
    [MemberData(nameof(SellCases))]
    public void SimulateSell_SingleQuoteFee(
        decimal quotePrice,
        decimal inputAmount,
        decimal pctBaseFee,
        decimal pctQuoteFee,
        decimal expectedQuoteGained)
    {
        TradeInput tradeInput = new TradeInput(AmountType.Absolute, inputAmount, false);
        Fee baseFee = new Fee(AmountType.Percentage, pctBaseFee, FeeSource.Base);
        Fee quoteFee = new Fee(AmountType.Percentage, pctQuoteFee, FeeSource.Quote);
        (decimal baseRemoved, decimal quoteGained, bool fullInputUsed) = TradeLogic.SimulateSell(
            quotePrice, inputAmount, tradeInput, new() { baseFee, quoteFee });

        Assert.True(fullInputUsed);
        Assert.Equal(inputAmount, baseRemoved); // Full input should be consumed
        Assert.Equal(expectedQuoteGained, quoteGained);
    }
}