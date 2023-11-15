using Game.Utils;
using UnityEngine;

namespace Game.ProjectileSystem
{
    public class ObjectSpawner : MonoBehaviour
    {
        public StringConstant surfaceTypeTag;
        public GameObject prefab;

        public void SpawnOnCollision(Collision other)
        {
            if (surfaceTypeTag != null && !other.gameObject.CompareTag(surfaceTypeTag.Value)) return;
            
            var spawnPosition = other.contacts[0].point + other.contacts[0].normal.normalized * 0.1f;
            var spawnRotation = Quaternion.LookRotation(other.contacts[0].normal * -1);
            
            var instance = Instantiate(prefab, spawnPosition, spawnRotation);
            instance.transform.SetParent(other.transform, true);
        }
    }
}