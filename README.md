# AlphaVantage Intraday Report API

A .NET 8 Web API that calls the Alpha Vantage Intraday (15-minute) endpoint, aggregates the last month of intraday data by day, and returns daily averages for high/low prices and total volume.

---

## How to Run

### 1. Install .NET 8 SDK

### 2. Add your Alpha Vantage API key  
Edit `appsettings.json`:

```json
{
  "AlphaVantage": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}

Or set an environment variable:

export AlphaVantage__ApiKey=YOUR_API_KEY

### 3. Run the API

From the project folder run: 
dotnet run

### 4. Call the Endpoint 
GET /api/AVClient-Intraday-Report?symbol=IBM

example: http://localhost:5165/api/AVClient-Intraday-Report?symbol=IBM


