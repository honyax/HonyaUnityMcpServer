using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HonyaUnityMcpServer.Prompts;

[McpServerPromptType]
public static class CreateGamePrompt
{
    [McpServerPrompt, Description("Unity でゲームを作る場合の手順")]
    public static string HowToCreateUnityGame()
    {
        Program.Logger.Log($"HowToCreateUnityGame");
        return "Unity でゲームを作る場合、以下の手順で作ると良いです。" +
            "- CreateGameObject, CreatePrimitiveGameObject で GameObject を生成" +
            "- SetTransform で Transform を設定" +
            "- CreateScript でスクリプトを作成" +
            "- AttachComponent でスクリプトや Camera などの Component を GameObject にアタッチ" +
            "- AssignComponentField で、作成したスクリプトの public フィールドに GameObject をアサイン";
    }
}
