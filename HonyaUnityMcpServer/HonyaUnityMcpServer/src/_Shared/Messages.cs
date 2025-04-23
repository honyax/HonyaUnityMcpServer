using System;

namespace HonyaMcp
{
    [Serializable]
    public class McpMessage
    {
        public string type;
        public string content;
        public string sender;
        public string timestamp;
    }

    public class Request { }
    public class Response
    {
        public bool result;
    }
    public class ErrorResponse : Response
    {
        public string message;
    }

    [Serializable]
    public class CreateGameObjectRequest : Request
    {
        public string name;
        public string parentName;
    }

    [Serializable]
    public class CreateGameObjectResponse : Response
    {
        public string name;
        public int instanceId;
    }

    public enum PrimitiveType
    {
        Empty,
        Sphere,
        Capsule,
        Cylinder,
        Cube,
        Plane,
        Quad,
    }

    [Serializable]
    public class CreatePrimitiveGameObjectRequest : Request
    {
        public PrimitiveType type;
        public string name;
        public string parentName;
    }
    [Serializable]
    public class CreatePrimitiveGameObjectResponse : Response
    {
        public string name;
        public int instanceId;
    }

    [Serializable]
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;
        public Vector3(float x_, float y_, float z_)
        {
            x = x_;
            y = y_;
            z = z_;
        }
        public override string ToString()
        {
            return $"x={x} y={y} z={z}";
        }
    }

    [Serializable]
    public class SetTransformRequest : Request
    {
        public int instanceId;
        public string name;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    [Serializable]
    public class SetTransformResponse : Response
    {
        public int instanceId;
    }

    [Serializable]
    public class CreateScriptRequest : Request
    {
        public string scriptFileName;
        public string sourceCode;
    }

    [Serializable]
    public class CreateScriptResponse : Response
    {
    }

    [Serializable]
    public class CreateScriptConfirmRequest : Request
    {
        public string scriptFileName;
    }

    [Serializable]
    public class CreateScriptConfirmResponse : Response
    {
        public bool compileFinished;
        public bool hasCompileError;
    }

    [Serializable]
    public class AttachComponentRequest : Request
    {
        public string componentName;
        public string gameObjectName;
    }

    [Serializable]
    public class AttachComponentResponse : Response
    {
    }

    [Serializable]
    public class AssignComponentFieldRequest : Request
    {
        public string targetGameObjectName;
        public string componentTypeName;
        public string fieldName;
        public string assignGameObjectName;
    }

    [Serializable]
    public class AssignComponentFieldResponse : Response
    {
    }
}
