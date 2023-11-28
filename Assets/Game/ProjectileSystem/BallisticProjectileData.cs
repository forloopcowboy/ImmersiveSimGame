using System;
using Game.EquipmentSystem;
using Game.HealthSystem;
using Game.Src.EventBusModule;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.ProjectileSystem
{
    [CreateAssetMenu(fileName = "Untitled Ballistic Projectile", menuName = "GameItem/Projectile/Projectile Data", order = 0)]
    public class BallisticProjectileData : AbstractProjectileData
    {
        public float launchSpeed;
        public bool autoRepool = true;
        [ShowIf("autoRepool")]
        public float autoRepoolDelay = 5f;

        public string InstanceId => ItemName; // todo: give a better ID
        
        /// <summary>
        /// Launches projectile from the given spawn point, with the given launch velocity,
        /// overriding the launch speed of the projectile data. Does not handle consumption.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="spawnPoint"></param>
        /// <param name="launchVelocity"></param>
        /// <returns></returns>
        public GameObject LaunchProjectile(Component user, Transform spawnPoint, Vector3 launchVelocity)
        {
            PreparePool(InstanceId);
            
            var projectile = GameObjectPool.Singleton.Get(InstanceId);
            
            var dmg = projectile.GetComponentInChildren<DamageComponent>();
            var userGameObj = user.gameObject;
            
            dmg.damageSource = userGameObj;
            
            // Ignore collisions between the projectile and the user (considering all colliders) for a brief period of time
            // Then re-enable collisions
            GenericCollisionHandler.IgnoreCollisionsForSeconds(userGameObj, projectile, 0.1f);
            
            projectile.transform.position = spawnPoint.position;
            projectile.transform.rotation = spawnPoint.rotation;
            
            var rb = projectile.GetComponentInChildren<Rigidbody>();
            if (rb) {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.AddForce(launchVelocity, ForceMode.VelocityChange);
            }
            
            if (autoRepool) GameObjectPool.Singleton.ReleaseIn(InstanceId, projectile, autoRepoolDelay);

            return projectile;
        }

        public override void Use(GameItemInventory user)
        {
            var spawnPoint = user.transform.FindTransformByName("ProjectileSpawnPoint");
            if (!spawnPoint)
                throw new NullReferenceException("ProjectileSpawnPoint not found on user. Please attach a ProjectileSpawnPoint to the user, and ensure it's forward vector indicates the desired launch direction.");
            
            LaunchProjectile(user, spawnPoint, spawnPoint.forward.normalized * launchSpeed);
            base.Use(user); // handle consumption
        }

        private void PreparePool(string instanceId)
        {
            GameObjectPool.Singleton.RegisterIfNotAlready(
                instanceId, // pooled by projectile data type
                () =>
                {
                    var instance = Instantiate(prefab);
                    instance.name = ItemName + $" (Pooled ID::{GameObjectPool.Singleton.Count(instanceId)})";
                    var pooledObject = instance.GetOrElseAddComponent<PooledObject>();
                    pooledObject.PoolCategory = instanceId;

                    return instance;
                },
                obj =>
                {
                    if (obj == null)
                    {
                        obj = Instantiate(prefab);
                        obj.name = ItemName + $" (Pooled ID::{GameObjectPool.Singleton.Count(instanceId)})";
                    }
                    
                    obj.transform.SetParent(null);
                    obj.SetActive(true);

                    var rb = obj.GetComponentInChildren<Rigidbody>();
                    if (rb)
                    {
                        if (!rb.isKinematic) rb.velocity = Vector3.zero;
                        rb.isKinematic = true;
                    }
                },
                obj =>
                {
                    obj.SetActive(false);

                    var rb = obj.GetComponentInChildren<Rigidbody>();
                    if (rb)
                    {
                        if (!rb.isKinematic) rb.velocity = Vector3.zero;
                        rb.isKinematic = true;
                    }

                    var dmg = obj.GetComponentInChildren<DamageComponent>();
                    dmg.damageSource = null;
                },
                Destroy,
                10
            );
        }
    }
}