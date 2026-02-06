using XtsApiClient.Services;

Console.WriteLine("PROGRAM STARTED");

// Create HTTP client
var client = HttpClientFactory.Create();

// LOGIN
var auth = new AuthService(client);
await auth.LoginAsync();

// 🚨 STOP EXECUTION IF LOGIN FAILED
if (string.IsNullOrEmpty(auth.Token))
{
    Console.WriteLine("Login failed. Market data & socket calls skipped.");
    return;
}

// MARKET DATA
var market = new MarketDataService(client);

// EQUITY OHLC
var ohlc = await market.GetEquityOhlcAsync(
    "HDFCBANK",
    "DAY",
    DateTime.Today.AddDays(-5),
    DateTime.Today
);

if (ohlc != null)
{
    Console.WriteLine("OHLC DATA:");
    Console.WriteLine(ohlc);
}

// F&O NEAR MONTH
var fno = await market.GetFnoNearMonthAsync("NIFTY");

if (fno != null)
{
    Console.WriteLine("FNO DATA:");
    Console.WriteLine(fno);
}

// Socket intentionally disabled until REST flow is stable
