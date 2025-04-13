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
