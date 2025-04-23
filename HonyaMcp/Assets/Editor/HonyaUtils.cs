using UnityEngine;

namespace HonyaMcp
{
    public static class HonyaUtils
    {
        public static GameObject FindObject(string name)
        {
            var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }
            return null;
        }
    }
}
