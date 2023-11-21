using System;
using Game.EquipmentSystem;
using Game.HealthSystem;
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

        public override void Use(GameItemInventory user)
        {
            var spawnPoint = user.transform.FindTransformByName("ProjectileSpawnPoint");
            if (!spawnPoint)
                throw new NullReferenceException("ProjectileSpawnPoint not found on user. Please attach a ProjectileSpawnPoint to the user, and ensure it's forward vector indicates the desired launch direction.");

            var instanceId = ItemName; // todo: use something better here
            
            GameObjectPool.Singleton.RegisterIfNotAlready(
                instanceId, // pooled by projectile data type
                () =>
                {
                    var instance = Instantiate(prefab);
                    instance.name = ItemName + $" (Pooled ID::{GameObjectPool.Singleton.Count(instanceId)})";

                    return instance;
                },
                obj =>
                {
                    obj.transform.SetParent(null);
                    obj.SetActive(true);
                    
                    var rb = obj.GetComponentInChildren<Rigidbody>();
                    if (rb) {
                      if (!rb.isKinematic) rb.velocity = Vector3.zero;
                      rb.isKinematic = true;
                      rb.detectCollisions = false;

                      // Prevent collisions with the user
                      GameObjectPool.Singleton.StartCoroutine(
                          CoroutineHelpers.DelayedAction(0.04f, () => rb.detectCollisions = true));
                    }
                    
                    var dmg = obj.GetComponentInChildren<DamageComponent>();
                    dmg.damageSource = user.gameObject;
                },
                obj =>
                {
                    obj.SetActive(false);
                    
                    var rb = obj.GetComponentInChildren<Rigidbody>();
                    if (rb) {
                      if (!rb.isKinematic) rb.velocity = Vector3.zero;
                      rb.isKinematic = true;
                      rb.detectCollisions = false;
                    }
                    
                    var dmg = obj.GetComponentInChildren<DamageComponent>();
                    dmg.damageSource = null;
                },
                Destroy,
                10
            );
            
            var projectile = GameObjectPool.Singleton.Get(instanceId);
            
            projectile.transform.position = spawnPoint.position;
            projectile.transform.rotation = spawnPoint.rotation;
            
            var rb = projectile.GetComponentInChildren<Rigidbody>();
            if (rb) {
              rb.isKinematic = false;
              rb.AddForce(spawnPoint.forward * launchSpeed, ForceMode.VelocityChange);
            }
            
            base.Use(user); // handle consumption
            
            if (autoRepool) GameObjectPool.Singleton.ReleaseIn(instanceId, projectile, autoRepoolDelay);
        }
    }
}