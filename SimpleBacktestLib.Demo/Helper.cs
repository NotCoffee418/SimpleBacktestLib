using CsvHelper;
using System.Globalization;
using System.IO.Compression;

namespace SimpleBacktestLib.Demo;

internal static class Helper
{
    public static async Task<bool> DownloadAndUnzipFile(string url, string filename)
    {
        if (File.Exists(filename))
        {
            Console.WriteLine("File already exists, skipping download");
            return true;
        }            

        var tempFile = Path.GetTempFileName();
        try
        {
            // Create a temp file to download the zip file
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var fileStream = new FileStream(tempFile, FileMode.Create))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                }
            }

            // Unzip the file
            using (var archive = ZipFile.OpenRead(tempFile))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Name != filename) continue;
                    entry.ExtractToFile(filename, true);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error downloading data: " + ex.Message);
            return false;
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    /// <summary>
    /// Quickly thrown together function to convert binance data. 
    /// I wouldn't recommend using this method in your code without heavily optimizing it.
    /// </summary>
    /// <param name="binanceDataCsvPath"></param>
    /// <returns></returns>
    public static async Task GenerateCandleDataFromBinance(string binanceDataCsvPath)
    {
        List<dynamic> rawBinanceData;
        using (var reader = new StreamReader(binanceDataCsvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                rawBinanceData = await csv.GetRecordsAsync<dynamic>().ToListAsync();

        long openTimeMs;
        List<BacktestCandle> btCandles = rawBinanceData
            .Where(x =>
            {
                openTimeMs = Convert.ToInt64(x.open_time);
                return openTimeMs > 1577833200000 && openTimeMs < 1609455600000;
            })
            .Select(x => new BacktestCandle
            {
                Open = Convert.ToDecimal(x.open, CultureInfo.InvariantCulture),
                Close = Convert.ToDecimal(x.close, CultureInfo.InvariantCulture),
                High = Convert.ToDecimal(x.high, CultureInfo.InvariantCulture),
                Low = Convert.ToDecimal(x.low, CultureInfo.InvariantCulture),
                Volume = Convert.ToDecimal(x.quote_volume, CultureInfo.InvariantCulture),
                Time = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(x.open_time)).DateTime
            }).ToList();

        using (var writer = new StreamWriter("sample-data.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<BacktestCandle>();
            csv.NextRecord();
            foreach (var record in btCandles)
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }
        }
        return;




    }
}
