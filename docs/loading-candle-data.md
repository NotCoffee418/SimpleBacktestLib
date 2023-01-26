# Loading Candle Data
SimpleBacktestLib does not have any built-in functionality to load candle data. You can use any method you like to load your data, as long as it is shaped as a list of [BacktestCandle](type-backtestcandle.md).

Below are some examples of how you can load your data.

## Loading from CSV

In this example we will use the nuget package [CsvHelper](https://www.nuget.org/packages/CsvHelper/) to load the candle data.:

### Loading from CSV with matching structure
```csharp
public async Task<List<BacktestCandle>> LoadFromCsvAsync(string filePath)
{
    using var reader = new StreamReader(filePath);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    return await csv.GetRecordsAsync<BacktestCandle>()
        .ToListAsync();
}

```

### Casting from different candle datatype
In case you're working with your own candle datatype with the same format, you can cast your data using the following code.
    
```csharp
public List<BacktestCandle> CastCandleData(List<MyCandleType> myCandles)
    => myCandles.Cast<BacktestCandle>().ToList();
```

### Converting from different structure 
In case you're working with your own format, you can cast your data using the following code.  
In this example, the property names don't match, so we need to manually map them.

```csharp

// in this example, the property names don't match
public List<BacktestCandle> MapCandleData(List<MyCandleType> myCandles)
    => myCandles.Select(candle => new BacktestCandle
    {
        Open = candle.Open,
        High = candle.High,
        Low = candle.Low,
        Close = candle.Close,
        Volume = candle.Volume,
        Time = candle.Timestamp /
    }).ToList()
```