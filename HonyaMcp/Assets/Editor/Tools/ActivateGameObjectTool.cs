using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Threading;

namespace HonyaMcp
{
    public class ActivateGameObjectTool : McpToolBase
    {
        public override string Name => "ActivateGameObject";

        public override Response Execute(string content)
        {
            var message = JsonUtility.FromJson<ActivateGameObjectRequest>(content);
            var gameObjectName = message.name;
            var active = message.active;
            var targetGo = HonyaUtils.FindObject(gameObjectName);
            var result = false;
            if (targetGo != null)
            {
                targetGo.SetActive(active);
                result = true;
            }
            return new ActivateGameObjectResponse { result = result };
        }
    }
}
