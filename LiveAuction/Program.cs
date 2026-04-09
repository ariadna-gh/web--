using System.Net.WebSockets;
using System.Text;
using LiveAuction;

var builder = WebApplication.CreateBuilder(args);

// Реєструємо ConnectionManager як singleton — один екземпляр на весь час роботи сервера
builder.Services.AddSingleton<ConnectionManager>();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseStaticFiles();
app.UseWebSockets(); // вмикаємо підтримку WebSocket

// GET /api/lots
app.MapGet("/api/lots", () => AuctionStore.Lots);

// GET /api/lots/{id}
app.MapGet("/api/lots/{id}", (int id) =>
{
    var lot = AuctionStore.Lots.FirstOrDefault(l => l.Id == id);
    return lot is null ? Results.NotFound() : Results.Ok(lot);
});

// POST /api/lots/{id}/bid — тепер після ставки робимо broadcast
app.MapPost("/api/lots/{id}/bid", async (int id, BidRequest bid, ConnectionManager cm) =>
{
    var lot = AuctionStore.Lots.FirstOrDefault(l => l.Id == id);

    if (lot is null)
        return Results.NotFound(new { error = "Лот не знайдено" });

    if (string.IsNullOrWhiteSpace(bid.Bidder))
        return Results.BadRequest(new { error = "Вкажіть ваше імʼя" });

    if (bid.Amount <= lot.CurrentPrice)
        return Results.BadRequest(new { error = $"Ставка має бути більше {lot.CurrentPrice}" });

    lot.CurrentPrice = bid.Amount;
    lot.TopBidder = bid.Bidder;

    // Повідомляємо всіх підключених клієнтів про нову ставку
    await cm.BroadcastAsync(new
    {
        type = "bid_update",
        lot
    });

    return Results.Ok(lot);
});

// GET /ws — точка підключення WebSocket
app.Map("/ws", async (HttpContext context, ConnectionManager cm) =>
{
    // Перевіряємо що це справді WebSocket-запит (з заголовком Upgrade: websocket)
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    var socket = await context.WebSockets.AcceptWebSocketAsync();
    var id = Guid.NewGuid();
    cm.Add(id, socket);
    await cm.BroadcastAsync(new
    {
    type = "viewer_update",
    count = cm.Count
    });

    Console.WriteLine($"[WS] Підключився клієнт {id}. Всього: {cm.Count}");

    // Чекаємо поки клієнт сам не закриє з'єднання
    var buffer = new byte[1024];
    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
            break;
    }

    cm.Remove(id);
    await cm.BroadcastAsync(new
    {
    type = "viewer_update",
    count = cm.Count
    });
    Console.WriteLine($"[WS] Відключився клієнт {id}. Всього: {cm.Count}");
});

app.Run();