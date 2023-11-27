using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.CollisionEffects
{
    public enum SpawnOrientation
    {
        CollisionNormal,
        CollisionNormalReversed,
        Absolute
    }
    
    /// <summary>
    /// Handles collision effects by outlining rules for spawning collision effects
    /// per CollisionMaterialType.
    /// </summary>
    public class CollisionEffectsController : SerializedMonoBehaviour
    {
        public List<CollisionRule> CollisionRules = new();
        public bool ShouldSpawnOnNextCollision = true;

        private void OnEnable()
        {
            ShouldSpawnOnNextCollision = true;
        }

        public void OnCollisionEnter(Collision other)
        {
            if (!ShouldSpawnOnNextCollision)
            {
                return;
            }
            
            var collisionMaterial = other.gameObject.GetComponent<CollisionMaterial>();
            if (collisionMaterial == null)
            {
                Debug.Log("No collision material found on " + other.gameObject.name + ".");
            }

            SpawnEffect(collisionMaterial == null ? CollisionMaterialType.Default : collisionMaterial.Type, other.GetContact(0).point, other.GetContact(0).normal, other.transform);
            ShouldSpawnOnNextCollision = false;
        }

        public void SpawnEffect(CollisionMaterialType collisionMaterialType, Vector3 position, Vector3 normal, Transform parent = null)
        {
            var rules = CollisionRules.FindAll(r => r.CollisionMaterialType.HasFlag(collisionMaterialType) || r.CollisionMaterialType == CollisionMaterialType.Any);

            foreach (var rule in rules)
            {
                Debug.Log("Executing spawn rule for " + rule.CollisionMaterialType + " at " + position + ".");
                if (rule.StickToCollisionObject)
                {
                    StickToCollidedObject(parent);
                }
                
                if (rule.SpawnEffects) foreach (var spawnRule in rule.SpawnRules)
                {
                    Vector3 spawnPosition;
                    Quaternion spawnRotation;

                    switch (spawnRule.SpawnOrientation)
                    {
                        case SpawnOrientation.CollisionNormal:
                            spawnPosition = position + normal.normalized * spawnRule.SpawnPositionOffset.magnitude;
                            spawnRotation = Quaternion.LookRotation(normal) * Quaternion.Euler(spawnRule.SpawnRotationOffset);
                            break;
                        case SpawnOrientation.CollisionNormalReversed:
                            spawnPosition = position + normal.normalized * spawnRule.SpawnPositionOffset.magnitude;
                            spawnRotation = Quaternion.LookRotation(normal * -1) * Quaternion.Euler(spawnRule.SpawnRotationOffset);
                            break;
                        case SpawnOrientation.Absolute:
                            spawnPosition = position + spawnRule.SpawnPositionOffset;
                            spawnRotation = Quaternion.Euler(spawnRule.SpawnRotationOffset);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                
                    var spawnedObject = Instantiate(spawnRule.Prefab, spawnPosition, spawnRotation);
                    
                    if (spawnRule.ReparentToCollisionObject && parent != null)
                    {
                        spawnedObject.transform.SetParent(parent, true);
                    }

                    if (spawnRule.SelfDestruct)
                    {
                        Destroy(spawnedObject, spawnRule.SelfDestructDelay);
                    }
                }
            }
        }
        
        public void StickToCollidedObject(Transform collidedObject)
        {
            var rb = GetComponentInChildren<Rigidbody>();
            
            if (rb != null)
            {
                rb.detectCollisions = false;
                rb.isKinematic = true;
                transform.SetParent(collidedObject.localScale != Vector3.one ? collidedObject.parent : collidedObject, true);
            }
        }
    }

    [Serializable]
    public class CollisionRule
    {
        public CollisionMaterialType CollisionMaterialType = CollisionMaterialType.Any;
        public bool SpawnEffects = true;
        [ShowIf("SpawnEffects")]
        public SpawnRule[] SpawnRules;
        public bool StickToCollisionObject = false;
        
    }
    
    [Serializable]
    public class SpawnRule
    {
        public GameObject Prefab;
        
        public SpawnOrientation SpawnOrientation;
        public Vector3 SpawnPositionOffset;
        public Vector3 SpawnRotationOffset;
        
        public bool ReparentToCollisionObject;
        
        public bool SelfDestruct;
        [ShowIf("SelfDestruct")]
        public float SelfDestructDelay;
    }
}