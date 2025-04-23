using ModelContextProtocol.Server;
using System.ComponentModel;
using HonyaUnityMcpServer.Libs;
using HonyaMcp;

namespace HonyaUnityMcpServer.Tools;

[McpServerToolType]
public static class AssignComponentFieldTool
{
    [McpServerTool, Description("コンポーネントのフィールドに GameObject にアサインする")]
    public static async Task<AssignComponentFieldResponse> HonyaAssignComponentField(
        [Description("コンポーネントがアタッチされている GameObject 名")]
        string targetGameObjectName,
        [Description("アタッチされているコンポーネントのタイプ名")]
        string componentTypeName,
        [Description("アサインするフィールド名")]
        string fieldName,
        [Description("アサインする GameObject 名")]
        string assignGameObjectName)
    {
        var response = await HonyaMcpClient.SendMessage<AssignComponentFieldRequest, AssignComponentFieldResponse>("AssignComponentField", new AssignComponentFieldRequest
        {
            targetGameObjectName = targetGameObjectName,
            componentTypeName = componentTypeName,
            fieldName = fieldName,
            assignGameObjectName = assignGameObjectName,
        });

        return response;
    }
}
