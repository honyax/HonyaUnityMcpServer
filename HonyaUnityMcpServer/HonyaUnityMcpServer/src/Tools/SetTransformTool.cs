using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class SetTransformTool
{
    [McpServerTool, Description("GameObjectのTransformを設定する")]
    public static async Task<SetTransformResponse> HonyaSetTransform(
        [Description("GameObjectのインスタンスID")]
        int instanceId,
        [Description("GameObjectの名前")]
        string name,
        [Description("アセットのX座標")]
        float posX = 0,
        [Description("アセットのY座標")]
        float posY = 0,
        [Description("アセットのZ座標")]
        float posZ = 0,
        [Description("アセットのX軸回転角度")]
        float eularAngleX = 0,
        [Description("アセットのY軸回転角度")]
        float eularAngleY = 0,
        [Description("アセットのZ軸回転角度")]
        float eularAngleZ = 0,
        [Description("アセットのX軸のスケール")]
        float scaleX = 1,
        [Description("アセットのY軸のスケール")]
        float scaleY = 1,
        [Description("アセットのZ軸のスケール")]
        float scaleZ = 1)
    {
        var response = await HonyaMcpClient.SendMessage<SetTransformRequest, SetTransformResponse>("SetTransform", new SetTransformRequest
        {
            instanceId = instanceId,
            name = name,
            position = new Vector3 { x = posX, y = posY, z = posZ },
            rotation = new Vector3 { x = eularAngleX, y = eularAngleY, z = eularAngleZ },
            scale = new Vector3 { x = scaleX, y = scaleY, z = scaleZ },
        });
        return response;
    }
}
