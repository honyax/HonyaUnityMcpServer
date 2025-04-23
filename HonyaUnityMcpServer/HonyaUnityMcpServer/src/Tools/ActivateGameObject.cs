using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class ActivateGameObjectTool
{
    [McpServerTool, Description("GameObject のアクティブ状態を設定する")]
    public static async Task<ActivateGameObjectResponse> HonyaActivateGameObject(
        [Description("アクティブ状態を設定する GameObject 名")]
        string name,
        [Description("true:アクティブにする、false:非アクティブにする")]
        bool active)
    {
        var response = await HonyaMcpClient.SendMessage<ActivateGameObjectRequest, ActivateGameObjectResponse>("ActivateGameObject", new ActivateGameObjectRequest
        {
            name = name,
            active = active
        });

        return response;
    }
}
