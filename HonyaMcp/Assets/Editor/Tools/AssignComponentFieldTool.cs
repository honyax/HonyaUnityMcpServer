using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace HonyaMcp
{
    public class AssignComponentFieldTool : McpToolBase
    {
        public override string Name => "AssignComponentField";

        public override Response Execute(string content)
        {
            var message = JsonUtility.FromJson<AssignComponentFieldRequest>(content);
            var targetGameObjectName = message.targetGameObjectName;
            var componentTypeName = message.componentTypeName;
            var fieldName = message.fieldName;
            var assignGameObjectName = message.assignGameObjectName;

            var targetObj = GameObject.Find(targetGameObjectName);
            if (targetObj == null)
            {
                var errorMessage = $"GameObject '{targetGameObjectName}' not found in scene.";
                Debug.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var assignObj = GameObject.Find(assignGameObjectName);
            if (assignObj == null)
            {
                var errorMessage = $"GameObject '{assignGameObjectName}' not found in scene.";
                Debug.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            try
            {
                Type componentType = null;

                // アセンブリ内の全ての型を検索
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    componentType = assembly.GetType(componentTypeName);
                    if (componentType != null)
                        break;
                }

                if (componentType == null)
                {
                    var errorMessage = $"Component type '{componentTypeName}' not found.";
                    Debug.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                var component = targetObj.GetComponent(componentType);
                if (component == null)
                {
                    var errorMessage = $"Component '{componentTypeName}' not found on '{targetGameObjectName}'.";
                    Debug.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                var field = componentType.GetField(fieldName,
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field == null)
                {
                    var errorMessage = $"Field '{fieldName}' not found in component '{componentTypeName}'.";
                    Debug.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                if (field.FieldType != typeof(Component))
                {
                    var errorMessage = $"Field '{fieldName}' is not of Component type.";
                    Debug.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                // Undoに登録
                Undo.RecordObject(component, "Assign Component Reference");

                // 値を設定
                field.SetValue(component, assignObj);

                // 変更をマーク
                EditorUtility.SetDirty(component);

                Debug.Log($"Successfully assigned {assignGameObjectName} to {fieldName} in {componentTypeName} on {targetGameObjectName}");

                return new AssignComponentFieldResponse { result = true };
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Error occurred: {e.Message}\n{e.StackTrace}");
                return new ErrorResponse { result = false, message = $"Failed to assign component: {e.Message}" };
            }
        }
    }
}
