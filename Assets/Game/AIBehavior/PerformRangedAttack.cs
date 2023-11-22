using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.EquipmentSystem;
using Game.ProjectileSystem;
using Game.Utils;
using UnityEngine;

namespace Game.AIBehavior
{
    [TaskName("Perform Ranged Attack")]
    [TaskDescription("Perform a ranged attack using equipped ranged weapons. This task will only work if the AI has a ranged weapon in their inventory. If the AI has multiple ranged weapons, the AI will use the first ranged weapon found in their inventory until it is depleted of ammo, then the AI will switch to the next ranged weapon in their inventory. If the AI has no ranged weapons in their inventory, this task will fail.")]
    public class PerformRangedAttack : Action
    {
        public SharedGameObject target;
        public SharedFloat projectileSpeed;
        
        private GameItemInventory _inventory;

        public override void OnStart()
        {
            if (_inventory == null)
            {
                _inventory = GetComponent<GameItemInventory>();
                projectileSpeed.Value = 3f;
            }
            
            if (_inventory == null) Debug.LogError("No GameItemInventory component found on " + gameObject.name + "!");
        }

        public override TaskStatus OnUpdate()
        {
            if (_inventory == null || target.Value == null) return TaskStatus.Failure;

            if (_inventory.TryGetItemOfType<AbstractProjectileData>(out var projectileItem))
            {
                projectileItem.Use(_inventory);
                
                return TaskStatus.Success;
            }
            
            return TaskStatus.Failure;
        }
    }
}