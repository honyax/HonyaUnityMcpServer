using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.Threading.Tasks;

namespace HonyaMcp
{
    public class CreateScriptConfirmTool : McpToolBase
    {
        public override string Name => "CreateScriptConfirm";
        public override bool IsAsync => true;

        private const string SCRIPTS_FOLDER = "Assets/Scripts";

        public override async Task<Response> ExecuteAsync(string content)
        {
            try
            {
                var message = JsonUtility.FromJson<CreateScriptConfirmRequest>(content);
                var scriptFileName = message.scriptFileName;

                // ファイル名に.csが含まれていない場合は追加
                if (!scriptFileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    scriptFileName += ".cs";
                }

                var fullPath = Path.Combine(SCRIPTS_FOLDER, scriptFileName);
                var assetPath = fullPath.Replace(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "").Replace('\\', '/');

                var compileFinished = !EditorApplication.isCompiling;
                var hasCompileError = false;

                if (compileFinished)
                {
                    // Compilation is not currently running, check the result
                    hasCompileError = CheckForCompileErrors(assetPath);
                }

                return new CreateScriptConfirmResponse
                {
                    result = true,
                    compileFinished = compileFinished,
                    hasCompileError = hasCompileError
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to check script compilation status: {ex.Message}\n{ex.StackTrace}");
                return new ErrorResponse
                {
                    result = false,
                    message = $"Failed to check script compilation status: {ex.Message}",
                };
            }
        }

        private bool CheckForCompileErrors(string assetPath)
        {
            // ステップ1: スクリプトがアセンブリに含まれているか確認
            var allAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player)
                                .Concat(CompilationPipeline.GetAssemblies(AssembliesType.Editor));

            var foundInAssembly = false;
            foreach (var assembly in allAssemblies)
            {
                if (assembly.sourceFiles.Contains(assetPath))
                {
                    foundInAssembly = true;
                    break;
                }
            }

            // スクリプトがどのアセンブリにも見つからない場合はエラー状態と見なす
            if (!foundInAssembly)
            {
                Debug.LogWarning($"Script {assetPath} not found in any compiled assembly. Assuming error or pending state.");
                return true;
            }

            // ステップ2: コンパイルエラーを検出する方法1 - AssetImporter.GetAtPath でインポーターが取得できない場合はエラーとする
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                Debug.LogWarning($"Cannot get AssetImporter for {assetPath}. Assuming error state.");
                return true;
            }

            // ステップ3: コンパイルエラーを検出する方法2 - MonoScript の読み込みができない場合はエラーとする
            var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
            if (monoScript == null)
            {
                Debug.LogWarning($"Cannot load MonoScript for {assetPath}. Assuming error state.");
                return true;
            }

            // MonoScriptからTypeを取得できるかチェック。Typeが取得できない場合は、コンパイルエラーの可能性が高い
            var scriptType = monoScript.GetClass();
            if (scriptType == null)
            {
                Debug.LogWarning($"Cannot get Type from MonoScript for {assetPath}. Likely a compilation error.");
                return true;
            }

            // ステップ4: コンパイルエラーを検出する方法3 - ログメッセージを確認
            // Unity 2019.3以降では、コンパイルエラーはConsoleウィンドウに表示されるだけでなく、
            // EditorUtility.DisplayDialog などでも表示される場合があります。
            // ここでは、スクリプトがアセンブリに含まれていて、MonoScriptからTypeが取得できれば
            // コンパイルは成功していると判断します。

            // すべてのチェックをパスした場合、コンパイルは成功していると判断
            return false;
        }
    }
}
