using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class CreateScriptTool
{
    [McpServerTool, Description("C#のソースコードのスクリプトファイルを作成する")]
    public static async Task<CreateScriptResponse> HumsCreateScript(
        [Description("C#のスクリプトファイル名")]
        string scriptFileName,
        [Description("スクリプトファイルのソースコード")]
        string sourceCode)
    {
        var response = await HonyaMcpClient.SendMessage<CreateScriptRequest, CreateScriptResponse>("CreateScript", new CreateScriptRequest
        {
            scriptFileName = scriptFileName,
            sourceCode = sourceCode,
        });
        return response;
    }
}
