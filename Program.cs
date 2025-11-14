var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<AlphaVantageClient>();

var app = builder.Build();

app.MapGet("/api/AVClient-Intraday-Report", async (
   string symbol,
    AlphaVantageClient client) => 
{
    if (string.IsNullOrWhiteSpace(symbol)){
        return Results.BadRequest("Missing symbol");
    }

    var candles = await client.GetInterday15Min(symbol);

    if (candles.Count == 0){
        return Results.NotFound("Missing symbol data");
    }

    var reports = client.BuildReports(candles);
    return Results.Ok(reports);
});

app.Run();