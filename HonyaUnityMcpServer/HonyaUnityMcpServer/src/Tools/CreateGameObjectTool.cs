using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class CreateGameObjectTool
{
    [McpServerTool, Description("GameObjectをシーンに追加する")]
    public static async Task<CreateGameObjectResponse> HonyaCreateGameObject(
        [Description("作成する GameObject の名前")]
        string name,
        [Description("親となる GameObject の名前。空文字列の場合は Root に作成する。")]
        string parentName
        )
    {
        var response = await HonyaMcpClient.SendMessage<CreateGameObjectRequest, CreateGameObjectResponse>("CreateGameObject", new CreateGameObjectRequest
        {
            name = name,
            parentName = parentName
        });
        return response;
    }
}
