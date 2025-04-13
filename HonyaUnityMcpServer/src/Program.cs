using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using System.Linq;
using System.Threading.Tasks;
using HonyaMcp;
using HonyaUnityMcpServer.Tools;
using System.Text.Json;

namespace HonyaUnityMcpServer;

public static class Program
{
    public const string SERVER_URL = "ws://localhost:8888/Mcp";
    private const string LOG_FILE = "D:/var/logs/honya_unity_mcp_server.log";

    private static Logger? _logger;
    public static Logger Logger
    {
        get
        {
            if (_logger == null)
            {
                _logger = new Logger(LOG_FILE);
            }
            return _logger;
        }
    }

    static async Task Main(string[] args)
    {
        // コマンドライン引数でクライアントテストを実行するかどうかを判定
        if (args.Contains("--run-client-test"))
        {
            Console.WriteLine("WebSocket クライアントテストを実行します...");
            await WebSocketClientTest.MainTest();
            Console.WriteLine("クライアントテストが終了しました。");
            return; // クライアントテストを実行したらサーバーは起動しない
        }
#if false
        var res = await CreatePrimitiveGameObjectTool.HumsCreatePrimitiveGameObject(PrimitiveType.Sphere, "MySphere");
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true,
        };
        _logger.Log($"resStr:{JsonSerializer.Serialize(res, options)}");
        var res2 = await SetTransformTool.HumsSetTransform(res.instanceId, 10, 20, 30, 0, 0, 0, 2, 3, 5);
        _logger.Log($"resStr2:{JsonSerializer.Serialize(res2, options)}");
#endif

        // --- 以下は通常のサーバー起動処理 ---

        var builder = Host.CreateApplicationBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });
        builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();

        var host = builder.Build();

        await host.RunAsync();
    }
}
