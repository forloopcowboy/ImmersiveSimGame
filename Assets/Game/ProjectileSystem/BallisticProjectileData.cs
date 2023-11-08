using System;
using Game.EquipmentSystem;
using Game.Utils;
using UnityEngine;

namespace Game.ProjectileSystem
{
    [CreateAssetMenu(fileName = "Untitled Ballistic Projectile", menuName = "GameItem/Projectile/Projectile Data", order = 0)]
    public class BallisticProjectileData : AbstractProjectileData
    {
        public float launchSpeed;

        public override void Use(GameItemInventory user)
        {
            var spawnPoint = user.transform.FindTransformByName("ProjectileSpawnPoint");
            if (!spawnPoint)
                throw new NullReferenceException("ProjectileSpawnPoint not found on user. Please attach a ProjectileSpawnPoint to the user, and ensure it's forward vector indicates the desired launch direction.");

            var instanceId = GetInstanceID().ToString();
            
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
                    obj.SetActive(true);
                    var rb = obj.GetComponentInChildren<Rigidbody>();
                    if (rb) {
                      rb.velocity = Vector3.zero;
                      rb.isKinematic = true;
                    }
                },
                obj =>
                {
                    obj.SetActive(false);
                    var rb = obj.GetComponentInChildren<Rigidbody>();
                    if (rb) {
                      rb.velocity = Vector3.zero;
                      rb.isKinematic = true;
                    }
                },
                Destroy
            );
            
            var projectile = GameObjectPool.Singleton.Get(instanceId);
            
            projectile.transform.position = spawnPoint.position;
            projectile.transform.rotation = spawnPoint.rotation;
            
            var rb = projectile.GetComponentInChildren<Rigidbody>();
            if (rb) {
              rb.isKinematic = false;
              rb.AddForce(spawnPoint.forward * launchSpeed, ForceMode.VelocityChange);
            }
            
            GameObjectPool.Singleton.ReleaseIn(instanceId, projectile, 45f);
        }
    }
}