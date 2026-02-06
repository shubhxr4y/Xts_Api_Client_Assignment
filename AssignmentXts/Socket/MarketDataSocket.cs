using System.Net.WebSockets;
using System.Text;

namespace XtsApiClient.Socket;

public class MarketDataSocket
{
    private ClientWebSocket _socket;

    public async Task ConnectAsync(string url)
    {
        _socket = new ClientWebSocket();
        await _socket.ConnectAsync(new Uri(url), CancellationToken.None);
        Console.WriteLine("✔ Socket connected");
    }

    public async Task SubscribeAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await _socket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }

    public async Task ReceiveAsync()
    {
        var buffer = new byte[4096];

        while (_socket.State == WebSocketState.Open)
        {
            var result = await _socket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            Console.WriteLine(
                Encoding.UTF8.GetString(buffer, 0, result.Count)
            );
        }
    }
}
