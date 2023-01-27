namespace SimpleBacktestLib.Demo;

internal class CustomDataModel
{
    public DateTime NextTradeTime { get; set; } = DateTime.MinValue;
    public TradeOperation NextTradeOperation { get; set; } = TradeOperation.Buy; // start with buy
}
