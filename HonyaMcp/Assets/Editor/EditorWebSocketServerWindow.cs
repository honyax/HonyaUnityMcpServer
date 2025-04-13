using UnityEngine;
using UnityEditor;

namespace HonyaMcp
{
    public class EditorWebSocketServerWindow : EditorWindow
    {
        private bool isServerRunning = false;
        private string statusMessage = "";
        private GUIStyle statusStyle;
        private Vector2 scrollPosition;

        // メニューからウィンドウを開く
        [MenuItem("Window/WebSocket Server")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<EditorWebSocketServerWindow>("WebSocket Server");
        }

        private void OnEnable()
        {
            // ウィンドウが有効になったときに状態を更新
            UpdateServerStatus();

            // エディタの更新時に状態を確認するためのコールバックを登録
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            // コールバックの登録解除
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            // サーバーの状態が変わったら表示を更新
            if (isServerRunning != HonyaWebSocketServer.IsListening)
            {
                UpdateServerStatus();
                Repaint();
            }
        }

        private void UpdateServerStatus()
        {
            isServerRunning = HonyaWebSocketServer.IsListening;
            statusMessage = isServerRunning ? "サーバー稼働中" : "サーバー停止中";
        }

        private void OnGUI()
        {
            // スタイルの初期化
            if (statusStyle == null)
            {
                statusStyle = new GUIStyle(EditorStyles.boldLabel);
                statusStyle.fontSize = 14;
            }

            // スクロール領域の開始
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // タイトル
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("WebSocket サーバー管理", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // サーバーの状態表示
            statusStyle.normal.textColor = isServerRunning ? Color.green : Color.red;
            EditorGUILayout.LabelField(statusMessage, statusStyle);
            EditorGUILayout.Space(5);

            // サーバーの起動・停止ボタン
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !isServerRunning;
            if (GUILayout.Button("サーバー起動", GUILayout.Height(30)))
            {
                HonyaWebSocketServer.StartServer();
                UpdateServerStatus();
            }
            GUI.enabled = isServerRunning;
            if (GUILayout.Button("サーバー停止", GUILayout.Height(30)))
            {
                HonyaWebSocketServer.StopServer();
                UpdateServerStatus();
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("WebSocketサーバーの状態を管理します。\n起動中はUnityエディタを閉じるまでサーバーが稼働します。", MessageType.Info);

            // スクロール領域の終了
            EditorGUILayout.EndScrollView();
        }
    }
}
