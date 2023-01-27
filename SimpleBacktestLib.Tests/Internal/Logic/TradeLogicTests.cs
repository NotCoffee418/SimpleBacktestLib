namespace SimpleBacktestLib.Tests.Internal.Logic;

public class TradeLogicTests
{
    [Theory]
    [InlineData(1000, 100, 0, 1, 0.099)]
    [InlineData(1000, 100, 1, 0, 0.099)]
    [InlineData(1000, 100, 1, 1, 0.098)]
    [InlineData(1000, 100, 0, 0, 0.1)]
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

    [Theory]
    [InlineData(1000, 0.1, 0, 1, 99)]
    [InlineData(1000, 0.1, 1, 0, 99)]
    [InlineData(1000, 0.1, 1, 1, 98)]
    [InlineData(1000, 0.1, 0, 0, 100)]
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
