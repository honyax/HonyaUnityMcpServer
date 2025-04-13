using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class CreateGameObjectTool
{
    [McpServerTool, Description("GameObjectをシーンに追加する")]
    public static async Task<CreateGameObjectResponse> HumsCreateGameObject(string name)
    {
        var response = await HonyaMcpClient.SendMessage<CreateGameObjectRequest, CreateGameObjectResponse>("CreateGameObject", new CreateGameObjectRequest
        {
            name = name
        });
        return response;
    }
}
