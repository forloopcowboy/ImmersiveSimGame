using UnityEngine;

namespace Game.Utils
{
    public class PooledObject : MonoBehaviour
    {
        public string PoolCategory = "default";
        
        public void ReturnToPool()
        {
            GameObjectPool.Singleton.Release(PoolCategory, gameObject);
        }
    }
}