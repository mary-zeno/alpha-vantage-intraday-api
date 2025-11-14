using System.Text.Json;
using System.Net.Http.Json;


public class AlphaVantageClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey; 

    public AlphaVantageClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["AlphaVantage:ApiKey"] ?? throw new InvalidOperationException("AlphaVantage:ApiKey not configured");
    }

    public async Task<Dictionary<string, Candle>> GetInterday15Min(string symbol)
    {
        var url = $"https://www.alphavantage.co/query" +
                  $"?function=TIME_SERIES_INTRADAY" +
                  $"&symbol={symbol}" +
                  $"&interval=15min" +
                  $"&outputsize=full" +
                  $"&apikey={_apiKey}";
        
        var response = await _http.GetFromJsonAsync<JsonElement>(url);

        if (!response.TryGetProperty("Time Series (15min)", out var timeSeries))
        {
            return new Dictionary<string, Candle>();
        }

        var dic = new Dictionary<string, Candle>();

        foreach(var k in timeSeries.EnumerateObject())
        {
            string date = k.Name;
            var candleVal = k.Value;
            var candle = new Candle
            {
                High = double.Parse(candleVal.GetProperty("2. high").GetString()!),
                Low = double.Parse(candleVal.GetProperty("3. low").GetString()!),
                Volume = long.Parse(candleVal.GetProperty("5. volume").GetString()!)
            };

            dic[date] = candle;
        }

        return dic;
    }

    public record Report(string Day, double LowAverage, double HighAverage, long Volume);

    public List<Report> BuildReports(Dictionary<string, Candle> candles)
    {
        var rep = candles.GroupBy(k => k.Key.Split(' ')[0]).Select(g =>
        {
            var highAves = g.Select(x => x.Value.High);
            var lowAves = g.Select(x => x.Value.Low);
            var volumes = g.Select(x => x.Value.Volume);

            return new Report(
                Day: g.Key,
                LowAverage: lowAves.Average(),
                HighAverage: highAves.Average(),
                Volume: volumes.Sum()
            );
        })
        .OrderBy(x => x.Day)
        .ToList();

        return rep;
    }


    public record Candle
    {
        public double High { get; init; }
        public double Low { get; init; }
        public long Volume { get; init; }
    }
}