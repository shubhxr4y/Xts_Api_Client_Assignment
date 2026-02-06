using System.Text;
using Newtonsoft.Json;

namespace XtsApiClient.Services;

public class MarketDataService
{
    private readonly HttpClient _client;

    public MarketDataService(HttpClient client)
    {
        _client = client;
    }

    public async Task<string?> GetEquityOhlcAsync(
        string symbol,
        string interval,
        DateTime from,
        DateTime to)
    {
        //  Convert dates to epoch seconds (REQUIRED by XTS API)
        long startEpoch = new DateTimeOffset(from).ToUnixTimeSeconds();
        long endEpoch = new DateTimeOffset(to).ToUnixTimeSeconds();

        //  Build correct OHLC URL
        string url =
            $"apimarketdata/instruments/ohlc?" +
            $"exchangeSegment=NSECM" +
            $"&exchangeInstrumentID={symbol}" +
            $"&startTime={startEpoch}" +
            $"&endTime={endEpoch}" +
            $"&compressionValue=DAY";

        var response = await _client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"OHLC request failed. StatusCode: {response.StatusCode}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string?> GetFnoNearMonthAsync(string underlying)
    {
        string url = "apimarketdata/instruments/quotes";

        var payload = new
        {
            instruments = new[]
            {
                new
                {
                    exchangeSegment = "NSEFO",
                    exchangeInstrumentID = underlying
                }
            },
            xtsMessageCode = 1502,
            publishFormat = "JSON"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(payload),
            Encoding.UTF8
        );
        content.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var response = await _client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"FNO request failed. StatusCode: {response.StatusCode}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return null;
        }

        return await response.Content.ReadAsStringAsync();
    }
}
