using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class CreateScriptTool
{
    [McpServerTool, Description("C#のソースコードのスクリプトファイルを作成する")]
    public static async Task<CreateScriptConfirmResponse> HumsCreateScript(
        [Description("C#のスクリプトファイル名")]
        string scriptFileName,
        [Description("スクリプトファイルのソースコード")]
        string sourceCode)
    {
        await HonyaMcpClient.SendMessage<CreateScriptRequest, CreateScriptResponse>("CreateScript", new CreateScriptRequest
        {
            scriptFileName = scriptFileName,
            sourceCode = sourceCode,
        });

        var response2 = new CreateScriptConfirmResponse
        {
            compileFinished = false,
            hasCompileError = false,
        };
        for (var i = 0; i < 10; i++)
        {
            await Task.Delay(5000);
            response2 = await HonyaMcpClient.SendMessage<CreateScriptConfirmRequest, CreateScriptConfirmResponse>("CreateScriptConfirm", new CreateScriptConfirmRequest
            {
                scriptFileName = scriptFileName
            });
            if (response2.compileFinished)
            {
                Console.WriteLine($"hasCompileError:{response2.hasCompileError}");
                break;
            }
        }

        return response2;
    }
}
