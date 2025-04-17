using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.Threading.Tasks;

namespace HonyaMcp
{
    public class CreateScriptTool : McpToolBase
    {
        public override string Name => "CreateScript";
        public override bool IsAsync => true;

        private const string SCRIPTS_FOLDER = "Assets/Scripts";
        private bool compilationInProgress = false;

        public override async Task<Response> ExecuteAsync(string content)
        {
            try
            {
                CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;

                var message = JsonUtility.FromJson<CreateScriptRequest>(content);
                var scriptFileName = message.scriptFileName;
                var sourceCode = message.sourceCode;

                // ファイル名に.csが含まれていない場合は追加
                if (!scriptFileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    scriptFileName += ".cs";
                }

                // フォルダが存在しない場合は作成
                if (!Directory.Exists(SCRIPTS_FOLDER))
                {
                    Directory.CreateDirectory(SCRIPTS_FOLDER);
                    AssetDatabase.Refresh();
                }

                var fullPath = Path.Combine(SCRIPTS_FOLDER, scriptFileName);
                File.WriteAllText(fullPath, sourceCode);
                AssetDatabase.Refresh();

#if false
                compilationInProgress = true;

                Debug.Log($"Created script file: {fullPath}");
                for (var i = 0; i < 1000; i++)
                {
                    await Task.Delay(100);
                    if (compilationInProgress)
                    {
                        Debug.Log($"Is Compiling... {i}");
                        continue;
                    }
                    break;
                }
#endif

                return new CreateScriptResponse
                {
                    result = true
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create script: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                CompilationPipeline.assemblyCompilationFinished -= OnCompilationFinished;
            }

            return new ErrorResponse
            {
                result = false,
                message = "Failed to create script",
            };
        }

        private void OnCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            Debug.Log($"OnCompilationFinished:{assemblyPath} {messages.Length}");
            var hasErrors = false;

            // コンパイル中フラグをリセット
            compilationInProgress = false;

            foreach (var message in messages)
            {
                if (message.type == CompilerMessageType.Error)
                {
                    hasErrors = true;
                    Debug.LogError($"コンパイルエラー: {message.message} at {message.file}:{message.line}");
                }
            }

            if (hasErrors)
            {
                Debug.LogError($"スクリプトのコンパイルに失敗しました");
            }
            else
            {
                Debug.Log($"スクリプトが正常にコンパイルされました");
            }
        }
    }
}
