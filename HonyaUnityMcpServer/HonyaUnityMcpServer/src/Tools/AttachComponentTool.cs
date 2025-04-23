using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class AttachComponentTool
{
    [McpServerTool, Description("コンポーネントを GameObject にアタッチする")]
    public static async Task<AttachComponentResponse> HonyaAttachComponent(
        [Description("アタッチするコンポーネント名")]
        string componentName,
        [Description("アタッチ先の GameObject 名")]
        string gameObjectName)
    {
        var response = await HonyaMcpClient.SendMessage<AttachComponentRequest, AttachComponentResponse>("AttachComponent", new AttachComponentRequest
        {
            componentName = componentName,
            gameObjectName = gameObjectName
        });

        return response;
    }
}
