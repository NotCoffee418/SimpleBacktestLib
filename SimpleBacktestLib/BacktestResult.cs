namespace SimpleBacktestLib;

public class BacktestResult
{    
    public List<BacktestTrade> SpotTrades { get; internal set; }    

    public List<MarginPosition> MarginTrades { get; internal set; }
}
