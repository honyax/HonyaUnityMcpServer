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
            Debug.Log($"AssignComponentField targetGameObjectName:{targetGameObjectName} componentTypeName:{componentTypeName} fieldName:{fieldName} assignGameObjectName:{assignGameObjectName}");

            var targetObj = HonyaUtils.FindObject(targetGameObjectName);
            if (targetObj == null)
            {
                var errorMessage = $"GameObject '{targetGameObjectName}' not found in scene.";
                Debug.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var assignObj = HonyaUtils.FindObject(assignGameObjectName);
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

                Debug.Log($"targetObj:{targetObj.name} GetComponent:{componentType}");
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

                // Undoに登録
                Undo.RecordObject(component, "Assign Component Reference");

                // 値を設定
                Debug.Log($"assignObj:{assignObj.name} GetComponent:{field.FieldType}");
                if (field.FieldType.IsArray)
                {
                    // 配列の要素タイプを取得
                    var elementType = field.FieldType.GetElementType();

                    // 現在のフィールド値（配列）を取得
                    Array currentArray = (Array)field.GetValue(component);

                    // 新しい配列を作成（サイズを1つ増やす）
                    Array newArray = Array.CreateInstance(elementType, currentArray != null ? currentArray.Length + 1 : 1);

                    // 既存の要素をコピー
                    if (currentArray != null)
                    {
                        Array.Copy(currentArray, newArray, currentArray.Length);
                    }

                    // 要素タイプのコンポーネントを取得
                    if (elementType == typeof(GameObject))
                    {
                        // 新しい要素を配列の最後に追加
                        newArray.SetValue(assignObj, newArray.Length - 1);
                    }
                    else
                    {
                        var elementComponent = assignObj.GetComponent(elementType);
                        if (elementComponent == null)
                        {
                            var errorMessage = $"Type '{elementType}' not found in component '{assignObj.name}'.";
                            Debug.LogError(errorMessage);
                            throw new Exception(errorMessage);
                        }

                        // 新しい要素を配列の最後に追加
                        newArray.SetValue(elementComponent, newArray.Length - 1);
                    }

                    // コンポーネントのフィールドに新しい配列を設定
                    field.SetValue(component, newArray);
                }
                else
                {
                    // 通常の処理（配列でない場合）
                    if (field.FieldType == typeof(GameObject))
                    {
                        field.SetValue(component, assignObj);
                    }
                    else
                    {
                        var assignComponent = assignObj.GetComponent(field.FieldType);
                        if (assignComponent == null)
                        {
                            var errorMessage = $"Type '{field.FieldType}' not found in component '{assignObj.name}'.";
                            Debug.LogError(errorMessage);
                            throw new Exception(errorMessage);
                        }

                        field.SetValue(component, assignComponent);
                    }
                }

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
