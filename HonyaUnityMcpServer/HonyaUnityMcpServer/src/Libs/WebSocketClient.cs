using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HonyaUnityMcpServer.Libs;

public class WebSocketClient : IDisposable
{
    private ClientWebSocket _clientWebSocket;
    private readonly Uri _serverUri;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public event EventHandler<string>? MessageReceived;
    public event EventHandler? ConnectionOpened;
    public event EventHandler? ConnectionClosed;
    public event EventHandler<Exception>? ErrorOccurred;

    public WebSocketClient(string serverUrl)
    {
        _serverUri = new Uri(serverUrl);
        _clientWebSocket = new ClientWebSocket();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task ConnectAsync()
    {
        try
        {
            _clientWebSocket = new ClientWebSocket(); // 再接続のために新しいインスタンスを作成
            await _clientWebSocket.ConnectAsync(_serverUri, _cancellationTokenSource.Token);
            ConnectionOpened?.Invoke(this, EventArgs.Empty);
            _ = ReceiveLoopAsync(_cancellationTokenSource.Token); // 受信ループを開始 (バックグラウンドで実行)
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
            await CloseInternalAsync(WebSocketCloseStatus.InternalServerError, $"Connection failed: {ex.Message}");
        }
    }

    public async Task SendAsync(string message)
    {
        if (_clientWebSocket.State != WebSocketState.Open)
        {
            ErrorOccurred?.Invoke(this, new InvalidOperationException("WebSocket is not open."));
            return;
        }

        try
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            await _clientWebSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
            await CloseInternalAsync(WebSocketCloseStatus.InternalServerError, $"Send failed: {ex.Message}");
        }
    }

    public async Task DisconnectAsync()
    {
        await CloseInternalAsync(WebSocketCloseStatus.NormalClosure, "Client requested disconnect.");
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4]; // 4KB buffer

        try
        {
            while (_clientWebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    MessageReceived?.Invoke(this, message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await CloseInternalAsync(WebSocketCloseStatus.NormalClosure, "Server initiated close.");
                    break; // ループを抜ける
                }
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセルされた場合は正常終了
            await CloseInternalAsync(WebSocketCloseStatus.NormalClosure, "Receive loop cancelled.");
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            // 接続が予期せず切断された場合
            ErrorOccurred?.Invoke(this, ex);
            await CloseInternalAsync(WebSocketCloseStatus.EndpointUnavailable, "Connection closed prematurely.");
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, ex);
            await CloseInternalAsync(WebSocketCloseStatus.InternalServerError, $"Receive loop error: {ex.Message}");
        }
        finally
        {
            // Ensure cleanup happens even if loop exits unexpectedly
            if (_clientWebSocket.State != WebSocketState.Closed && _clientWebSocket.State != WebSocketState.Aborted)
            {
                await CloseInternalAsync(WebSocketCloseStatus.InternalServerError, "Receive loop terminated unexpectedly.");
            }
        }
    }

    private async Task CloseInternalAsync(WebSocketCloseStatus closeStatus, string statusDescription)
    {
        if (_clientWebSocket.State == WebSocketState.Open || _clientWebSocket.State == WebSocketState.CloseReceived || _clientWebSocket.State == WebSocketState.CloseSent)
        {
            try
            {
                // キャンセルトークンをキャンセルして受信ループを停止させる
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }
                // タイムアウト付きでクローズ処理を試みる
                var closeTask = _clientWebSocket.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
                // 5秒待っても完了しない場合はタイムアウト
                if (await Task.WhenAny(closeTask, Task.Delay(TimeSpan.FromSeconds(5))) != closeTask)
                {
                    // タイムアウトした場合は強制終了
                    _clientWebSocket.Abort();
                    ErrorOccurred?.Invoke(this, new TimeoutException("WebSocket close timed out. Aborting connection."));
                }
            }
            catch (ObjectDisposedException)
            {
                // すでに破棄されている場合は無視
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, new Exception($"Error during WebSocket close: {ex.Message}", ex));
                // クローズ中にエラーが発生した場合でも Abort を試みる
                try { _clientWebSocket.Abort(); } catch { /* Abort中のエラーは無視 */ }
            }
        }
        else if (_clientWebSocket.State != WebSocketState.Closed) // Open, CloseReceived, CloseSent 以外で Closed でもない場合
        {
            try { _clientWebSocket.Abort(); } catch { /* Abort中のエラーは無視 */ }
        }

        // 状態に関わらず破棄し、イベントを発火
        _clientWebSocket.Dispose();
        ConnectionClosed?.Invoke(this, EventArgs.Empty);
    }

    // Dispose pattern for proper resource cleanup
    private bool _disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                _cancellationTokenSource.Cancel(); // Ensure loop stops
                                                   // CloseInternalAsync を同期的に待つのはデッドロックのリスクがあるため、ここでは Dispose のみを呼ぶ
                _clientWebSocket?.Dispose();
                _cancellationTokenSource?.Dispose();
            }
            _disposed = true;
        }
    }

    ~WebSocketClient()
    {
        Dispose(false);
    }
}
