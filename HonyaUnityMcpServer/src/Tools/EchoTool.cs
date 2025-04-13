using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HonyaUnityMcpServer.Tools
{
    [McpServerToolType]
    public static class EchoTool
    {
        [McpServerTool, Description("Echoes the message back to the client.")]
        public static string Echo(string message)
        {
            var ret = $"hello {message}";
            Program.Logger.Log($"入力:{message}");
            Program.Logger.Log($"出力:{ret}");
            return ret;
        }
    }
}
