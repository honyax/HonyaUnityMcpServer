using System;
using UnityEngine;
using WebSocketSharp;

namespace HonyaMcp
{
    public class CreatePrimitiveGameObjectTool : McpToolBase
    {
        public override string Name => "CreatePrimitiveGameObject";

        public override Response Execute(string content)
        {
            try
            {
                var message = JsonUtility.FromJson<CreatePrimitiveGameObjectRequest>(content);
                GameObject go = null;
                string createdName = "";

                switch (message.type)
                {
                    case PrimitiveType.Empty:
                        createdName = string.IsNullOrEmpty(message.name) ? "GameObject" : message.name;
                        go = new GameObject(createdName);
                        break;

                    case PrimitiveType.Sphere:
                        createdName = string.IsNullOrEmpty(message.name) ? "Sphere" : message.name;
                        go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Sphere);
                        go.name = createdName;
                        break;
                    case PrimitiveType.Cube:
                        createdName = string.IsNullOrEmpty(message.name) ? "Cube" : message.name;
                        go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
                        go.name = createdName;
                        break;
                    case PrimitiveType.Capsule:
                        createdName = string.IsNullOrEmpty(message.name) ? "Capsule" : message.name;
                        go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Capsule);
                        go.name = createdName;
                        break;
                    case PrimitiveType.Cylinder:
                        createdName = string.IsNullOrEmpty(message.name) ? "Cylinder" : message.name;
                        go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cylinder);
                        go.name = createdName;
                        break;
                    case PrimitiveType.Plane:
                        createdName = string.IsNullOrEmpty(message.name) ? "Plane" : message.name;
                        go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Plane);
                        go.name = createdName;
                        break;
                    case PrimitiveType.Quad:
                        createdName = string.IsNullOrEmpty(message.name) ? "Quad" : message.name;
                        go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Quad);
                        go.name = createdName;
                        break;
                    default:
                        Debug.LogWarning($"Invalid PrimitiveType: {message.type}");
                        return new ErrorResponse
                        {
                            result = false,
                            message = $"Invalid PrimitiveType specified: {message.type}",
                        };
                }

                if (go != null)
                {
                    if (!message.parentName.IsNullOrEmpty())
                    {
                        var parentGo = GameObject.Find(message.parentName);
                        if (parentGo != null)
                        {
                            go.transform.parent = parentGo.transform;
                        }
                    }
                    Debug.Log($"Created GameObject: {go.name} of type {message.type} on main thread");
                    return new CreatePrimitiveGameObjectResponse
                    {
                        result = true,
                        name = go.name,
                        instanceId = go.GetInstanceID()
                    };
                }
                else
                {
                    return new ErrorResponse
                    {
                        result = false,
                        message = $"Failed to create GameObject for unknown reason. Type: {message.type}",
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed during GameObject creation: {ex.Message}\n{ex.StackTrace}");
            }
            return new Response
            {
                result = false
            };
        }
    }
}
