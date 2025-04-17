using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace HonyaMcp
{
    public class AttachComponentTool : McpToolBase
    {
        public override string Name => "AttachComponent";

        public override Response Execute(string content)
        {
            try
            {
                var message = JsonUtility.FromJson<AttachComponentRequest>(content);
                var componentName = message.componentName;
                var gameObjectName = message.gameObjectName;

                if (string.IsNullOrEmpty(gameObjectName))
                {
                    Debug.LogError("GameObject name is not specified.");
                    return new ErrorResponse { result = false, message = "GameObject name is not specified." };
                }
                if (string.IsNullOrEmpty(componentName))
                {
                    Debug.LogError("Component name is not specified.");
                    return new ErrorResponse { result = false, message = "Component name is not specified." };
                }

                var gameObject = GameObject.Find(gameObjectName);
                if (gameObject == null)
                {
                    Debug.LogError($"GameObject '{gameObjectName}' not found.");
                    return new ErrorResponse { result = false, message = $"GameObject '{gameObjectName}' not found." };
                }

                // Find the component type. This might need adjustment depending on assembly qualification.
                // First, try finding in common assemblies (UnityEngine, Assembly-CSharp)
                var componentType = Type.GetType($"UnityEngine.{componentName}, UnityEngine") ??
                                    Type.GetType($"{componentName}, Assembly-CSharp");

                // If not found, search through all loaded assemblies (less efficient)
                if (componentType == null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        componentType = assembly.GetType(componentName);
                        if (componentType != null && typeof(Component).IsAssignableFrom(componentType))
                        {
                            break; // Found a suitable component type
                        }
                        // Also check nested within UnityEngine namespace in other assemblies if applicable
                        componentType = assembly.GetType($"UnityEngine.{componentName}");
                        if (componentType != null && typeof(Component).IsAssignableFrom(componentType))
                        {
                            break; // Found a suitable component type
                        }
                    }
                }


                if (componentType == null)
                {
                    Debug.LogError($"Component type '{componentName}' not found or is not a valid Component.");
                    return new ErrorResponse { result = false, message = $"Component type '{componentName}' not found or is not a valid Component." };
                }

                if (!typeof(Component).IsAssignableFrom(componentType))
                {
                    Debug.LogError($"Type '{componentName}' is not derived from UnityEngine.Component.");
                    return new ErrorResponse { result = false, message = $"Type '{componentName}' is not derived from UnityEngine.Component." };
                }

                // Check if the component already exists
                if (gameObject.GetComponent(componentType) != null)
                {
                    Debug.LogWarning($"Component '{componentName}' already exists on GameObject '{gameObjectName}'.");
                    // Consider this a success or failure based on requirements. Let's treat it as success for now.
                    return new AttachComponentResponse { result = true };
                }

                // Add the component
                var addedComponent = gameObject.AddComponent(componentType);
                if (addedComponent == null)
                {
                    Debug.LogError($"Failed to add component '{componentName}' to GameObject '{gameObjectName}'. AddComponent returned null.");
                    return new ErrorResponse { result = false, message = $"Failed to add component '{componentName}' to GameObject '{gameObjectName}'. AddComponent returned null." };
                }

                Debug.Log($"Successfully attached component '{componentName}' to GameObject '{gameObjectName}'.");
                return new AttachComponentResponse { result = true };
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError($"Failed to attach component: {ex.Message}\n{ex.StackTrace}");
                return new ErrorResponse { result = false, message = $"Failed to attach component: {ex.Message}" };
            }
        }
    }
}
