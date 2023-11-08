using Game.EquipmentSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.ProjectileSystem
{
    public abstract class AbstractProjectileData : EquipableItemType
    {
        [Required("Projectile prefab is required.")]
        public GameObject prefab;
    }
}