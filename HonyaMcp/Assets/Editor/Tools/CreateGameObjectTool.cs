using System;
using UnityEngine;
using WebSocketSharp;

namespace HonyaMcp
{
    public class CreateGameObjectTool : McpToolBase
    {
        public override string Name => "CreateGameObject";

        public override Response Execute(string content)
        {
            try
            {
                var message = JsonUtility.FromJson<CreatePrimitiveGameObjectRequest>(content);
                var createdName = string.IsNullOrEmpty(message.name) ? "GameObject" : message.name;
                var go = new GameObject(createdName);
                if (!message.parentName.IsNullOrEmpty())
                {
                    var parentGo = GameObject.Find(message.parentName);
                    if (parentGo != null)
                    {
                        go.transform.parent = parentGo.transform;
                    }
                }
                Debug.Log($"Created GameObject:{go.name} on main thread");
                return new CreateGameObjectResponse
                {
                    result = true,
                    name = go.name,
                    instanceId = go.GetInstanceID()
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to create GameObject: {ex.Message}");
            }

            return new Response
            {
                result = false
            };
        }
    }
}
