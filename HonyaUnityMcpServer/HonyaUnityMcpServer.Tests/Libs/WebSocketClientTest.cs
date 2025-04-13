using Xunit.Abstractions;
using HonyaUnityMcpServer.Libs;

namespace HonyaUnityMcpServer.Tests;

public class WebSocketClientTest
{
    private readonly ITestOutputHelper _output;

    public WebSocketClientTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task MainTest()
    {
        // WebSocketサーバーのアドレス (WebSocketServer.cs と合わせる)
        string serverUrl = "ws://localhost:8888/Chat";
        using var client = new WebSocketClient(serverUrl);

        // イベントハンドラの設定
        client.ConnectionOpened += (sender, e) => _output.WriteLine("WebSocket: 接続しました。");
        client.ConnectionClosed += (sender, e) => _output.WriteLine("WebSocket: 切断しました。");
        client.MessageReceived += (sender, message) => _output.WriteLine($"WebSocket 受信: {message}");
        client.ErrorOccurred += (sender, ex) => _output.WriteLine($"WebSocket エラー: {ex.Message}");

        _output.WriteLine($"WebSocket サーバー ({serverUrl}) に接続します...");
        await client.ConnectAsync();

        var message = "WebSocketClientTest";
        _output.WriteLine($"WebSocket 送信: {message}");
        await client.SendAsync(message);
        _output.WriteLine("切断しています...");
        await client.DisconnectAsync();
        _output.WriteLine("プログラムを終了します。");
    }
}
