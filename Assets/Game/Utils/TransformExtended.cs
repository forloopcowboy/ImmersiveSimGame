using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class TransformExtended
    {
        public static Transform FindTransformByName(this Transform root, string transformName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                Transform current = queue.Dequeue();

                if (current.name == transformName)
                {
                    return current;
                }

                foreach (Transform child in current)
                {
                    queue.Enqueue(child);
                }
            }

            Debug.Log("Transform not found with name: " + transformName);
            return null;
        }
        
        public static TComponent GetOrElseAddComponent<TComponent> (this GameObject gObj)
            where TComponent : UnityEngine.Component
        {
            TComponent c = gObj.GetComponent<TComponent>();
            if (c == null) c = gObj.gameObject.AddComponent<TComponent>();

            return c;
        }
        
        public static TComponent GetOrElseAddComponent<TComponent> (this Component gObj)
            where TComponent : UnityEngine.Component
        {
            TComponent c = gObj.GetComponent<TComponent>();
            if (c == null) c = gObj.gameObject.AddComponent<TComponent>();

            return c;
        }
        
        public static TComponent GetOrElseAddComponent<TComponent> (this Transform gObj)
            where TComponent : UnityEngine.Component
        {
            TComponent c = gObj.GetComponent<TComponent>();
            if (c == null) c = gObj.gameObject.AddComponent<TComponent>();

            return c;
        }
    }
}