using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class CreatePrimitiveGameObjectTool
{
    [McpServerTool, Description("PrimitiveなGameObjectをシーンに追加する")]
    public static async Task<CreatePrimitiveGameObjectResponse> HumsCreatePrimitiveGameObject(PrimitiveType type, string name)
    {
        var response = await HonyaMcpClient.SendMessage<CreatePrimitiveGameObjectRequest, CreatePrimitiveGameObjectResponse>("CreatePrimitiveGameObject", new CreatePrimitiveGameObjectRequest
        {
            type = type,
            name = name
        });
        return response;
    }
}
