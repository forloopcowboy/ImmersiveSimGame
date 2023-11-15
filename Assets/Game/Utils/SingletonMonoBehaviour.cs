using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Utils
{
    public class SingletonMonoBehaviour<T> : SerializedMonoBehaviour
        where T : Component
    {
        public static bool IsInitialized = false;
        

        private static T _instance;
        public static T Singleton
        {
            get
            {
                if (_instance == null)
                {
                    var objs = FindObjectsOfType(typeof(T)) as T[];
                    if (objs.Length > 0)
                        _instance = objs[0];
                    if (objs.Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = string.Format("Global_{0}", typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}