using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class CreatePrimitiveGameObjectTool
{
    [McpServerTool, Description("PrimitiveなGameObjectをシーンに追加する")]
    public static async Task<CreatePrimitiveGameObjectResponse> HonyaCreatePrimitiveGameObject(
        [Description("作成する Primitive な GameObject のタイプ")]
        PrimitiveType type,
        [Description("作成する GameObject の名前")]
        string name,
        [Description("親となる GameObject の名前。空文字列の場合は Root に作成する。")]
        string parentName)
    {
        var response = await HonyaMcpClient.SendMessage<CreatePrimitiveGameObjectRequest, CreatePrimitiveGameObjectResponse>("CreatePrimitiveGameObject", new CreatePrimitiveGameObjectRequest
        {
            type = type,
            name = name,
            parentName = parentName
        });
        return response;
    }
}
