using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using XtsApiClient.Config;

namespace XtsApiClient.Services;

// Handles authentication and token setup for XTS API
public class AuthService
{
    private readonly HttpClient _client;

    public string Token { get; private set; }

    public AuthService(HttpClient client)
    {
        _client = client;
    }

    // Logs in to the API and stores the auth token
    public async Task LoginAsync()
    {
        var payload = new
        {
            appKey = XtsConfig.ApiKey,
            secretKey = XtsConfig.ApiSecret,
            source = XtsConfig.ApiSource
        };

        var response = await _client.PostAsync(
            "https://xts.rmoneyindia.co.in:3000/apimarketdata/auth/login",
            new StringContent(
                JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                "application/json"
            )
        );

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Login failed. StatusCode: {response.StatusCode}");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return;
        }


        dynamic json = JsonConvert.DeserializeObject(
            await response.Content.ReadAsStringAsync()
        );
        
        // Save token and attach it to future requests
        Token = json.result.token;
        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", Token);

        Console.WriteLine("✔ Login successful");
    }
}
