using System.Net.Http;

namespace XtsApiClient.Services;

public static class HttpClientFactory
{
    public static HttpClient Create()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(Config.XtsConfig.BaseUrl);
        return client;
    }
}
