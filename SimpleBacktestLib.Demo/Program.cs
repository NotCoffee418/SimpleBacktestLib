using CsvHelper;
using SimpleBacktestLib;
using SimpleBacktestLib.Demo;
using System.Globalization;
using System.Text;

// Load candle data
List<BacktestCandle> candleData;
using (var reader = new StreamReader("sample-data.csv"))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        candleData = await csv.GetRecordsAsync<BacktestCandle>().ToListAsync();


// Configure the backtest
BacktestBuilder builder = BacktestBuilder.CreateBuilder(candleData)
    .WithQuoteBudget(5000)
    .WithInitialCustomData(new CustomDataModel())
    .OnTick(ProcessTick)        // Pass strategy logic on to the ProcessTick method                         
    .OnLogEntry((entry, _) =>   // Write all log entries to the console
    {
        Console.WriteLine(entry.ToString());
    });

// Run the backtest
Console.WriteLine("Running backtest...");
BacktestResult result = await builder.RunAsync();
PrintResult(result);
Console.ReadLine();


// Define a strategy where we Arbitrarily buy and sell every 3 hours
void ProcessTick(BacktestState state)
{
    var custData = state.GetCustomData<CustomDataModel>();

    // Not time to trade yet
    if (state.GetCurrentCandle().Time < custData.NextTradeTime)
        return;

    // Buy or sell depending on the last action
    if (custData.NextTradeOperation == TradeOperation.Buy)
        state.Trade.Spot.Buy();
    else
        state.Trade.Spot.Sell();
}

// Output the results in our own preferred format
void PrintResult(BacktestResult r)
{
    // Formatting
    StringBuilder sb = new("Backtest Results:");
    Action<string, string> addLn = (string label, string value) =>
        sb.AppendLine((label + ':').PadLeft(20) + value);

    // Add data we want
    addLn("P/L $", r.TotalProfitInQuote.ToString());
    addLn("Trade Count", r.SpotTrades.Count.ToString());
    addLn("Strategy Profit Ratio", r.ProfitRatio.ToString());
    addLn("Buy & Hold Profit Ratio", r.BuyAndHoldProfitRatio.ToString());
    addLn("Days Evaluated", r.EvaluatedCandleTimespan().TotalDays.ToString());
}