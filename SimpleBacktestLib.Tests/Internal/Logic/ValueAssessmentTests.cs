namespace SimpleBacktestLib.Tests.Internal.Logic;

public class ValueAssessmentTests
{
    [Theory]
    [InlineData(AssetType.Quote, 0.6, 600, 1000, 1200)]
    [InlineData(AssetType.Base, 0.6, 600, 1000, 1.2)]
    public void GetCombinedValue(AssetType asset, decimal baseAmount, decimal quoteAmount, decimal quotePrice, decimal expected)
    {
        var actual = ValueAssessment.GetCombinedValue(asset, baseAmount, quoteAmount, quotePrice);
        Assert.Equal(expected, actual);
    }
}
