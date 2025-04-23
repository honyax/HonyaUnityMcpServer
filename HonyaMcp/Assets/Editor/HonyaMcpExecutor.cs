using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HonyaMcp
{
    public class HonyaMcpExecutor
    {
        private static HonyaMcpExecutor _instance = new();
        public static HonyaMcpExecutor Instance => _instance;

        private Dictionary<string, McpToolBase> _tools = new();

        private HonyaMcpExecutor()
        {
            RegisterTools();
        }
        private void RegisterTools()
        {
            McpToolBase tool;
            tool = new CreateGameObjectTool();
            _tools.Add(tool.Name, tool);
            tool = new CreatePrimitiveGameObjectTool();
            _tools.Add(tool.Name, tool);
            tool = new SetTransformTool();
            _tools.Add(tool.Name, tool);
            tool = new CreateScriptTool();
            _tools.Add(tool.Name, tool);
            tool = new CreateScriptConfirmTool();
            _tools.Add(tool.Name, tool);
            tool = new AttachComponentTool();
            _tools.Add(tool.Name, tool);
            tool = new AssignComponentFieldTool();
            _tools.Add(tool.Name, tool);
            tool = new ActivateGameObjectTool();
            _tools.Add(tool.Name, tool);
        }

        public void Execute(McpMessage message, Action<Response> onCompleted)
        {
            if (_tools.TryGetValue(message.type, out var tool))
            {
                if (tool.IsAsync)
                {
                    EditorApplication.delayCall += async () =>
                    {
                        var result = await tool.ExecuteAsync(message.content);
                        onCompleted(result);
                    };
                }
                else
                {
                    EditorApplication.delayCall += () =>
                    {
                        var result = tool.Execute(message.content);
                        onCompleted(result);
                    };
                }
            }
            else
            {
                Debug.LogWarning($"Unknown ToolType:{message.type}");
                onCompleted(new ErrorResponse
                {
                    result = false,
                    message = $"Unknown ToolType:{message.type}"
                });
            }
        }
    }
}
