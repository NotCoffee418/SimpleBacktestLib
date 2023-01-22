using SimpleBacktestLib.Internal.Models;
using System.ComponentModel;

namespace SimpleBacktestLib.Tests.Internal.Margin;

public class MarginLogicTests
{
    [Theory]
    [InlineData(0, 100, 1100, 1, 110)] // Profitable long 1x, quote only liquidity
    [InlineData(0, 100, 900, 1, 90)] // Unprofitable long 1x, quote only liquidity
    [InlineData(0, 100, 1100, 2, 120)] // Profitable long 2x, quote only liquidity
    [InlineData(0, 100, 900, 2, 80)] // Unprofitable long 2x, quote only liquidity
    [InlineData(0.1, 0, 1100, 1, 120)] // Profitable long 1x, base only liquidity
    [InlineData(0.1, 0, 900, 1, 80)] // Unprofitable long 1x, base only liquidity
    [InlineData(0.1, 0, 1100, 2, 130)] // Profitable long 2x, base only liquidity
    [InlineData(0.1, 0, 900, 2, 70)] // Unprofitable long 2x, base only liquidity
    [InlineData(0.05, 50, 1100, 1, 115)] // Profitable long 1x, combined liquidity
    [InlineData(0.05, 50, 900, 1, 85)] // Unprofitable long 1x, combined liquidity
    [InlineData(0.05, 50, 1100, 2, 125)] // Profitable long 2x, combined liquidity
    [InlineData(0.05, 50, 900, 2, 75)] // Unprofitable long 2x, combined liquidity
    public void CalculateUnrealizedBalances_LongScenarios(
        decimal baseCollateral,
        decimal quoteCollateral,
        decimal tickPrice, 
        decimal leverageRatio, 
        decimal expectedCombinedQuote)
    {
        MarginPosition longPos =
            MarginPosition.GeneratePosition(
            TradeType.MarginLong,
            1000,
            new TradeInput(AmountType.Max, 0),
            baseCollateral,
            quoteCollateral,
            leverageRatio,
            0.1m);
        
        // Calculate
        (bool isLiquid, decimal updatedBase, decimal updatedQuote) 
            = MarginLogic.CalculateUnrealizedBalances(longPos, tickPrice, baseCollateral, quoteCollateral);
        decimal actualCombinedQuote = ValueAssessment.GetCombinedValue(AssetType.Quote, updatedBase, updatedQuote, tickPrice);
        decimal roundedActualCombinedQuote = Math.Round(actualCombinedQuote, 4);

        // Assert
        Assert.True(isLiquid);
        Assert.Equal(expectedCombinedQuote, roundedActualCombinedQuote);
        Assert.True(updatedBase >= 0);
        Assert.True(updatedQuote >= 0);
    }

    [Theory]
    [InlineData(0, 100, 1100, 1, 90)] // Unprofitable short 1x, quote only liquidity
    [InlineData(0, 100, 900, 1, 110)] // Profitable short 1x, quote only liquidity
    [InlineData(0, 100, 1100, 2, 80)] // Unprofitable short 2x, quote only liquidity
    [InlineData(0, 100, 900, 2, 120)] // Profitable short 2x, quote only liquidity
    [InlineData(0.1, 0, 1100, 1, 100)] // Unprofitable short 1x, base only liquidity
    [InlineData(0.1, 0, 900, 1, 100)] // Profitable short 1x, base only liquidity
    [InlineData(0.1, 0, 1100, 2, 90)] // Unprofitable short 2x, base only liquidity
    [InlineData(0.1, 0, 900, 2, 110)] // Profitable short 2x, base only liquidity
    [InlineData(0.05, 50, 1100, 1, 95)] // Unprofitable short 1x, combined liquidity
    [InlineData(0.05, 50, 900, 1, 105)] // Profitable short 1x, combined liquidity
    [InlineData(0.05, 50, 1100, 2, 85)] // Unprofitable short 2x, combined liquidity
    [InlineData(0.05, 50, 900, 2, 115)] // Profitable short 2x, combined liquidity
    public void CalculateUnrealizedBalances_ShortScenarios(
        decimal baseCollateral,
        decimal quoteCollateral,
        decimal tickPrice,
        decimal leverageRatio,
        decimal expectedCombinedQuote)
    {
        MarginPosition shortPos =
            MarginPosition.GeneratePosition(
            TradeType.MarginShort,
            1000,
            new TradeInput(AmountType.Max, 0),
            baseCollateral,
            quoteCollateral,
            leverageRatio,
            0.1m);

        // Calculate
        (bool isLiquid, decimal updatedBase, decimal updatedQuote)
            = MarginLogic.CalculateUnrealizedBalances(shortPos, tickPrice, baseCollateral, quoteCollateral);
        decimal actualCombinedQuote = ValueAssessment.GetCombinedValue(AssetType.Quote, updatedBase, updatedQuote, tickPrice);
        decimal roundedActualCombinedQuote = Math.Round(actualCombinedQuote, 4);

        // Assert
        Assert.True(isLiquid);
        Assert.Equal(expectedCombinedQuote, roundedActualCombinedQuote);
        Assert.True(updatedBase >= 0);
        Assert.True(updatedQuote >= 0);
    }

    [Fact]
    public void CalculateUnrealizedBalances_Illiquid()
    {
        decimal baseCollateral = 0;
        decimal quoteCollateral = 100;
        MarginPosition longPos =
            MarginPosition.GeneratePosition(
            TradeType.MarginLong,
            1000,
            new TradeInput(AmountType.Max, 0),
            0,
            quoteCollateral,
            10,
            0.1m);

        // Calculate
        (bool isLiquid, decimal updatedBase, decimal updatedQuote)
            = MarginLogic.CalculateUnrealizedBalances(longPos, 800, baseCollateral, quoteCollateral);
        decimal actualCombinedQuote = ValueAssessment.GetCombinedValue(AssetType.Quote, updatedBase, updatedQuote, 900);
        decimal roundedActualCombinedQuote = Math.Round(actualCombinedQuote, 4);

        // Assert
        Assert.False(isLiquid);
        Assert.Equal(-100m, roundedActualCombinedQuote);
        Assert.Equal(-100m, updatedQuote);
        Assert.True(updatedBase == 0);
    }
}
