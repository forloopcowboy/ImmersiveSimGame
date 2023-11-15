using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Utils
{
     public class GameObjectPool : SingletonMonoBehaviour<GameObjectPool>
    {
        private static readonly string defaultCategory = "default";
        private Dictionary<string, ObjectPool<GameObject>> cache = new();

        private void Awake()
        {
            Register(defaultCategory, () => new GameObject("PooledObject"));
        }

        /// <summary>
        /// Returns the number of active objects in the pool.
        /// </summary>
        public int Count(string category)
        {
            return cache[category].CountActive;
        }

        public void Remove(string category)
        {
            if (cache.ContainsKey(category))
            {
                cache[category].Clear();
                cache[category] = null;
            }
        }

        public void RegisterIfNotAlready(
            string category,
            Func<GameObject> createFunc,
            Action<GameObject> actionOnGet = null,
            Action<GameObject> actionOnRelease = null,
            Action<GameObject> actionOnDestroy = null,
            int maxSize = 150
        )
        {
            if (!cache.ContainsKey(category)) 
                Register(category, createFunc, actionOnGet, actionOnRelease, actionOnDestroy, maxSize);
        }
        
        /// <summary>
        /// If a pool is already registered under the category, the previous pool will be cleared.
        /// </summary>
        public void Register(
            string category,
            Func<GameObject> createFunc,
            Action<GameObject> actionOnGet = null,
            Action<GameObject> actionOnRelease = null,
            Action<GameObject> actionOnDestroy = null,
            int maxSize = 150
        ) {
            Remove(category);
            
            cache.Add(
                category,
                new ObjectPool<GameObject>(
                    createFunc,
                    actionOnGet,
                    actionOnRelease,
                    actionOnDestroy,
                    maxSize: maxSize
                )
            );
        }
        
        public GameObject Get(string category)
        {
            if (category != null && cache.ContainsKey(category)) return cache[category].Get();

            throw new IndexOutOfRangeException(
                $"Could not find a pool for category {category}. Please register it using the Register() method.");
        }

        public void Release(string category, GameObject obj)
        {
            if (category != null && cache.ContainsKey(category))
            {
                cache[category].Release(obj);
            } else throw new IndexOutOfRangeException(
                $"Could not find a pool for category {category}. Please register it using the Register() method.");
        }

        public Coroutine ReleaseIn(string category, GameObject obj, float delay = 30f)
        {
            return StartCoroutine(
                CoroutineHelpers.DelayedAction(
                    delay,
                    () =>
                    {
                        try
                        {
                            // object might already be released at this point, so we catch the potential exception from this
                            Release(category, obj);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e.Message);
                        }
                    })
                );
        }
        
        public Coroutine ReleaseWhen(
            string category,
            GameObject obj,
            Func<bool> condition
        )
        {
            return StartCoroutine(
                ReleaseWhenCoroutine(category, obj, condition)
            );
        }

        private IEnumerator ReleaseWhenCoroutine(
            string category,
            GameObject obj,
            Func<bool> condition
        )
        {
            if (condition()) TryRelease(category, obj);
            else yield return new WaitUntil(condition);
            TryRelease(category, obj);
        }

        private void TryRelease(string category, GameObject obj)
        {
            try
            {
                // object might already be released at this point, so we catch the potential exception from this
                Release(category, obj);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }
}