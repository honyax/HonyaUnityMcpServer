using System;
using System.Threading.Tasks;
using HonyaUnityMcpServer.Libs; // WebSocketClientクラスの名前空間

public class WebSocketClientTest
{
    public static async Task MainTest() // Program.csから呼び出せるように public static に変更
    {
        // WebSocketサーバーのアドレス (WebSocketServer.cs と合わせる)
        string serverUrl = "ws://localhost:8888/Chat";
        using var client = new WebSocketClient(serverUrl);

        // イベントハンドラの設定
        client.ConnectionOpened += (sender, e) => Console.WriteLine("WebSocket: 接続しました。");
        client.ConnectionClosed += (sender, e) => Console.WriteLine("WebSocket: 切断しました。");
        client.MessageReceived += (sender, message) => Console.WriteLine($"WebSocket 受信: {message}");
        client.ErrorOccurred += (sender, ex) => Console.WriteLine($"WebSocket エラー: {ex.Message}");

        Console.WriteLine($"WebSocket サーバー ({serverUrl}) に接続します...");
        await client.ConnectAsync();

        Console.WriteLine("接続が確立されました。メッセージを入力して Enter キーを押すと送信します。");
        Console.WriteLine("'exit' と入力すると切断して終了します。");

        string? input;
        while ((input = Console.ReadLine()) != null && input.ToLower() != "exit")
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                await client.SendAsync(input);
                Console.WriteLine($"WebSocket 送信: {input}");
            }
        }

        Console.WriteLine("切断しています...");
        await client.DisconnectAsync();
        Console.WriteLine("プログラムを終了します。");
    }

    // Program.cs から直接実行する場合のエントリーポイント (オプション)
    // public static async Task Main(string[] args)
    // {
    //     await MainTest();
    // }
}
