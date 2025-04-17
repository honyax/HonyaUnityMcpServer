using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HonyaMcp;

namespace HonyaUnityMcpServer.Libs;

public class HonyaMcpClient
{
    private static readonly TimeSpan ResponseTimeout = TimeSpan.FromSeconds(60); // タイムアウト時間を設定 (例: 60秒)
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        IncludeFields = true,
        WriteIndented = true,
    };

    // TResponse ジェネリック型パラメータを追加
    public static async Task<TResponse> SendMessage<TRequest, TResponse>(string type, TRequest request)
        where TRequest : Request
        where TResponse : Response // TResponse が Response を継承することを制約
    {
        var ws = new WebSocketClient(Program.SERVER_URL);
        // TaskCompletionSource の型を TResponse に変更
        var tcs = new TaskCompletionSource<TResponse>();

        EventHandler<string>? messageHandler = null;
        EventHandler<Exception>? errorHandler = null;
        EventHandler? closeHandler = null;

        messageHandler = (sender, message) =>
        {
            try
            {
                // ここで受信したメッセージが期待する応答か確認する
                // 例: JSONをパースして特定のフィールドを確認
                // if (IsExpectedResponse(message, requestId))
                // {
                //    tcs.TrySetResult(message); // 応答データをセット
                // }

                // ★★★ 重要 ★★★
                // 現在の実装では、単純に最初に受信したメッセージを応答として扱います。
                // 実際には、応答メッセージの形式を定義し、
                // どのリクエストに対する応答かを識別する仕組みが必要です。
                // (例: requestId を使う、メッセージタイプで判別するなど)
                Program.Logger.Log($"Message Received:{message}");
                var mcpMessage = JsonSerializer.Deserialize<McpMessage>(message, options);
                // デシリアライズを TResponse 型で行う
                var response = JsonSerializer.Deserialize<TResponse>(mcpMessage.content, options);
                // ★★★ 注意 ★★★
                // エラー応答 (ErrorResponse など) を適切に処理するロジックが必要になる場合があります。
                // 現状は成功応答と同じ型 (TResponse) にデシリアライズしようとします。
                tcs.TrySetResult(response);
            }
            catch (JsonException jsonEx)
            {
                // JSON パースエラーの場合、エラー応答を生成して返す試み (オプション)
                // もし TResponse が ErrorResponse を許容するなら、以下のような処理が可能
                // if (typeof(TResponse).IsAssignableFrom(typeof(ErrorResponse))) {
                //     var errorResponse = new ErrorResponse { result = false, message = $"Failed to parse response JSON: {jsonEx.Message}" };
                //     tcs.TrySetResult((TResponse)(Response)errorResponse); // キャストが必要
                // } else {
                //     tcs.TrySetException(new Exception($"Failed to parse response JSON: {jsonEx.Message}", jsonEx));
                // }
                // 現状は例外をそのまま設定
                tcs.TrySetException(new Exception($"Failed to parse response JSON: {jsonEx.Message}", jsonEx));
            }
            catch (Exception ex)
            {
                tcs.TrySetException(new Exception($"Error processing WebSocket message: {ex.Message}", ex));
            }
            finally
            {
                // 一度応答を受け取ったらハンドラを解除
                if (messageHandler != null) ws.MessageReceived -= messageHandler;
                if (errorHandler != null) ws.ErrorOccurred -= errorHandler;
                if (closeHandler != null) ws.ConnectionClosed -= closeHandler;
            }
        };

        errorHandler = (sender, ex) =>
        {
            Program.Logger.Log($"Error Occurerd:{sender}");
            tcs.TrySetException(new Exception($"WebSocket error occurred: {ex.Message}", ex));
            // エラー発生時もハンドラを解除
            if (messageHandler != null) ws.MessageReceived -= messageHandler;
            if (errorHandler != null) ws.ErrorOccurred -= errorHandler;
            if (closeHandler != null) ws.ConnectionClosed -= closeHandler;
        };

        closeHandler = (sender, args) =>
        {
            Program.Logger.Log($"Connection Closed:{sender}");
            tcs.TrySetException(new Exception("WebSocket connection closed before receiving response."));
            // 接続切断時もハンドラを解除
            if (messageHandler != null) ws.MessageReceived -= messageHandler;
            if (errorHandler != null) ws.ErrorOccurred -= errorHandler;
            if (closeHandler != null) ws.ConnectionClosed -= closeHandler;
        };

        ws.MessageReceived += messageHandler;
        ws.ErrorOccurred += errorHandler;
        ws.ConnectionClosed += closeHandler;

        try
        {
            var message = new McpMessage
            {
                type = type,
                content = JsonSerializer.Serialize(request, options),
                sender = "client",
                timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            var jsonMessage = JsonSerializer.Serialize(message, options);
            Program.Logger.Log($"Content:{message.content}");
            Program.Logger.Log($"Sent Message:{jsonMessage}");

            await ws.ConnectAsync();
            await ws.SendAsync(jsonMessage);

            // タイムアウト付きで応答を待つ
            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(ResponseTimeout));

            if (completedTask == tcs.Task)
            {
                // tcs.Task が正常に完了した場合 (エラーを含む)
                return await tcs.Task; // TResponse 型の結果または例外を取得
            }
            else
            {
                // タイムアウトした場合
                throw new TimeoutException($"Did not receive WebSocket response within {ResponseTimeout.TotalSeconds} seconds.");
            }
        }
        catch (Exception ex)
        {
            // SendAsync や待機中にエラーが発生した場合
            Program.Logger.Log($"Error during WebSocket send/wait: {ex.Message}");
            // 例外を再スローして呼び出し元で処理させる
            throw;
        }
        finally
        {
            Program.Logger.Log($"HumsCreateGameObject DisconnectAsync");
            await ws.DisconnectAsync();

            // 念のため、ここでもハンドラを解除
            ws.MessageReceived -= messageHandler;
            ws.ErrorOccurred -= errorHandler;
            ws.ConnectionClosed -= closeHandler;
        }
    }
}
