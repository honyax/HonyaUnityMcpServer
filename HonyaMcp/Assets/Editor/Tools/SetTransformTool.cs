using System;
using UnityEngine;
using UnityEditor;

namespace HonyaMcp
{
    public class SetTransformTool : McpToolBase
    {
        public override string Name => "SetTransform";

        public override Response Execute(string content)
        {
            try
            {
                var message = JsonUtility.FromJson<SetTransformRequest>(content);
                var instanceId = message.instanceId;
                var name = message.name;

                var obj = EditorUtility.InstanceIDToObject(instanceId);
                GameObject go = obj as GameObject;

                if (go == null && !string.IsNullOrEmpty(name))
                {
                    go = GameObject.Find(name);
                    if (go != null)
                    {
                        Debug.Log($"GameObject with name '{name}' found instead of InstanceID {instanceId}.");
                    }
                }

                if (go != null)
                {
                    var t = go.transform;
                    var pos = message.position;
                    var rot = message.rotation;
                    var scale = message.scale;
                    Debug.Log($"SetTransformTool t:{t} pos:{pos} rot:{rot} scale:{scale} content:{content}");
                    t.position = new UnityEngine.Vector3(pos.x, pos.y, pos.z);
                    t.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
                    t.localScale = new UnityEngine.Vector3(scale.x, scale.y, scale.z);
                    return new SetTransformResponse
                    {
                        result = true,
                        instanceId = go.GetInstanceID()
                    };
                }
                else
                {
                    Debug.LogWarning($"GameObject with InstanceID {instanceId} and name '{name}' not found.");
                    return new ErrorResponse
                    {
                        result = false,
                        message = $"GameObject with InstanceID {instanceId} and name '{name}' not found.",
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed during SetTransform: {ex.Message}\n{ex.StackTrace}");
            }

            return new ErrorResponse
            {
                result = false,
                message = "Failed to SetTransform",
            };
        }
    }
}
