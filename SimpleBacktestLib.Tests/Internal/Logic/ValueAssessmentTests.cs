namespace SimpleBacktestLib.Tests.Internal.Logic;

public class ValueAssessmentTests
{
    public static TheoryData<AssetType, decimal, decimal, decimal, decimal> Cases => new()
    {
        { AssetType.Quote, 0.6m, 600m, 1000m, 1200m },
        { AssetType.Base,  0.6m, 600m, 1000m, 1.2m },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void GetCombinedValue(AssetType asset, decimal baseAmount, decimal quoteAmount, decimal quotePrice, decimal expected)
    {
        var actual = ValueAssessment.GetCombinedValue(asset, baseAmount, quoteAmount, quotePrice);
        Assert.Equal(expected, actual);
    }
}