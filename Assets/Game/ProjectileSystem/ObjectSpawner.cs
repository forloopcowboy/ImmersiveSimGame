using Game.Utils;
using UnityEngine;

namespace Game.ProjectileSystem
{
    public class ObjectSpawner : MonoBehaviour
    {
        public StringConstant surfaceTypeTag;
        public GameObject prefab;
        public float destroyDelay = 2f;
        
        public void SpawnOnCollision(Collision other)
        {
            if (surfaceTypeTag != null && !other.gameObject.CompareTag(surfaceTypeTag.Value)) return;
            
            var spawnPosition = other.contacts[0].point;
            var spawnRotation = Quaternion.LookRotation(other.contacts[0].normal * -1);
            var spawnedObject = Instantiate(prefab, spawnPosition, spawnRotation);
            Destroy(spawnedObject, destroyDelay);
        }
    }
}