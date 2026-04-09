using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace LiveAuction;

// Зберігає всі активні WebSocket-з'єднання і вміє надсилати їм повідомлення
public class ConnectionManager

{
    // ConcurrentDictionary — потокобезпечний словник.
    // Guid — унікальний ключ для кожного підключення.
    private readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();

    public void Add(Guid id, WebSocket socket) => _sockets[id] = socket;

    public void Remove(Guid id) => _sockets.TryRemove(id, out _);
    
    public int Count => _sockets.Count;

    // Надсилає повідомлення всім підключеним клієнтам
    public async Task BroadcastAsync(object message)
    {
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);

        foreach (var (id, socket) in _sockets)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
