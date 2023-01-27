using CsvHelper;
using SimpleBacktestLib;
using SimpleBacktestLib.Demo;
using System.Globalization;
using System.Text;


// Download trading data (sample BTCUSD from binance)
string dateFilename = "btcusd-sample.csv";
string sourceUrl = "https://notcoffee418-github-demo-data.s3.eu-central-1.amazonaws.com/btcusd-sample.zip";
bool gotData = File.Exists(dateFilename);
while (!gotData)
{
    Console.WriteLine("Downloading sample data...");
    gotData = await Helper.DownloadAndUnzipFile(sourceUrl, dateFilename);
}

// Load candle data
Console.WriteLine("Configuring Backtest...");
List<BacktestCandle> candleData;
using (var reader = new StreamReader(dateFilename))
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
    DateTime currentCandleTime = state.GetCurrentCandle().Time;
    if (currentCandleTime < custData.NextTradeTime)
        return;

    // Buy or sell depending on the last action
    if (custData.NextTradeOperation == TradeOperation.Buy)
    {
        state.Trade.Spot.Buy();
        custData.NextTradeOperation = TradeOperation.Sell;
    }
    else
    {
        state.Trade.Spot.Sell();
        custData.NextTradeOperation = TradeOperation.Buy;
    }
    custData.NextTradeTime = currentCandleTime.AddHours(3);
}

// Output the results in our own preferred format
void PrintResult(BacktestResult r)
{
    // Formatting
    StringBuilder sb = new(Environment.NewLine);
    sb.AppendLine("Backtest Results:");
    sb.AppendLine();
    Action<string, string> addLn = (string label, string value) =>
        sb.AppendLine((label + ':').PadRight(30) + value);

    // Add data we want
    addLn("Days Evaluated", r.EvaluatedCandleTimespan().TotalDays.ToString());
    addLn("Trade Count", r.SpotTrades.Count.ToString());
    sb.AppendLine();
    addLn("P/L $", r.TotalProfitInQuote.ToString());
    addLn("Profit Strategy", $"{Math.Round(r.ProfitRatio * 100, 2)}%");
    addLn("Profit Buy & Hold", $"{Math.Round(r.BuyAndHoldProfitRatio * 100, 2)}%");
    sb.AppendLine();
    addLn("Final Balance Base", r.FinalBaseBudget.ToString());
    addLn("Final Balance Quote", r.FinalQuoteBudget.ToString());



    // Print it
    Console.WriteLine(sb.ToString());
}