using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonyaUnityMcpServer.Libs;

public class Logger
{
    private readonly string _logFilePath;
    private readonly object _lockObject = new object();

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;

        // ログファイルのディレクトリが存在しない場合は作成
        var directoryPath = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    // 同期的にログを書き込む
    public void Log(string message, string category = "INFO")
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logEntry = $"[{timestamp}] [{category}] {message}";

        lock (_lockObject)
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine(logEntry);
                writer.Flush();
            }
        }

        // オプション: デバッグ中はコンソールにも出力
        // Console.WriteLine(logEntry);
    }

    // 非同期にログを書き込む
    public async Task LogAsync(string message, string category = "INFO")
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logEntry = $"[{timestamp}] [{category}] {message}";

        await using (StreamWriter writer = new StreamWriter(_logFilePath, true))
        {
            await writer.WriteLineAsync(logEntry);
            await writer.FlushAsync();
        }
    }

    // 標準入力をログに記録
    public void LogInput(string input)
    {
        Log(input, "INPUT");
    }

    // 標準出力をログに記録
    public void LogOutput(string output)
    {
        Log(output, "OUTPUT");
    }
}
