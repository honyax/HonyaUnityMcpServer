using UnityEngine;
using UnityEditor;
using WebSocketSharp;
using WebSocketSharp.Server;
using System;

namespace HonyaMcp
{
    [InitializeOnLoad]
    public class HonyaWebSocketServer
    {
        private static WebSocketServer wssv;

        // サーバーの状態を外部から確認するためのプロパティ
        public static bool IsListening => wssv != null && wssv.IsListening;

        static HonyaWebSocketServer()
        {
            StartServer();
            EditorApplication.quitting += StopServer;
        }

        public static void StartServer()
        {
            if (wssv != null && wssv.IsListening)
            {
                Debug.Log("WebSocket Server is already running.");
                return;
            }

            // ポート番号は任意で変更可能
            int port = 8888;
            wssv = new WebSocketServer($"ws://localhost:{port}");

            // サービスを追加
            wssv.AddWebSocketService<Chat>("/Chat");
            wssv.AddWebSocketService<McpHandler>("/Mcp");

            try
            {
                wssv.Start();
                if (wssv.IsListening)
                {
                    Debug.Log($"WebSocket Server started on port {port}. Listening on ws://localhost:{port}/Chat and ws://localhost:{port}/Mcp");
                }
                else
                {
                    Debug.LogError("WebSocket Server failed to start.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"WebSocket Server failed to start: {ex.Message}");
                wssv = null; // 失敗したらnullに戻す
            }
        }

        public static void StopServer()
        {
            if (wssv != null && wssv.IsListening)
            {
                wssv.Stop();
                Debug.Log("WebSocket Server stopped.");
            }
            wssv = null; // 参照をクリア
        }
    }

    // WebSocketの振る舞いを定義するクラス
    public class Chat : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Debug.Log($"Received message: {e.Data}");
            // 受け取ったメッセージをそのまま返す (エコーサーバー)
            Send($"Echo: {e.Data}");
            // 全員にブロードキャストする場合
            // Sessions.Broadcast($"Broadcast: {e.Data}");
        }

        protected override void OnOpen()
        {
            Debug.Log("WebSocket connection opened.");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Debug.Log($"WebSocket connection closed: {e.Reason}");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Debug.LogError($"WebSocket error: {e.Message}");
        }
    }

    // Mcp用のWebSocketハンドラ
    public class McpHandler : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Debug.Log($"Received MCP message: {e.Data}");
            try
            {
                // JSONメッセージをMcpMessageオブジェクトにデシリアライズ
                McpMessage message = JsonUtility.FromJson<McpMessage>(e.Data);
                Debug.Log($"Message Received:{message.type}");
                HonyaMcpExecutor.Instance.Execute(message, (result) =>
                {
                    var response = new McpMessage
                    {
                        type = "response",
                        content = JsonUtility.ToJson(result),
                        sender = "server",
                        timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    Send(JsonUtility.ToJson(response));
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError($"Error processing MCP message: {ex.Message}");
                // エラーメッセージを返す
                McpMessage errorResponse = new McpMessage
                {
                    type = "error",
                    content = JsonUtility.ToJson(new ErrorResponse
                    {
                        result = false,
                        message = ex.Message
                    }),
                    sender = "server",
                    timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                Send(JsonUtility.ToJson(errorResponse));
            }
        }

        protected override void OnOpen()
        {
            Debug.Log("MCP WebSocket connection opened.");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Debug.Log($"MCP WebSocket connection closed: {e.Reason}");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            Debug.LogError($"MCP WebSocket error: {e.Message}");
        }
    }
}
