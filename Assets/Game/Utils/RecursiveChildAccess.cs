using UnityEngine;

namespace Game.Utils
{
    public static class RecursiveChildAccess
    {
        public static void ForEachChild(this Transform transform, System.Action<Transform> action)
        {
            foreach (Transform child in transform)
            {
                action(child);
                child.ForEachChild(action);
            }
        }
    }
}