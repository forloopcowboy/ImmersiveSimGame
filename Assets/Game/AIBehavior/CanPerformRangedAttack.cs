using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.EquipmentSystem;
using Game.ProjectileSystem;
using UnityEngine;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if the entity can perform a ranged attack, otherwise returns OnFailure.")]
    public class CanPerformRangedAttack : Conditional
    {
        public SharedGameObject target;
        public TaskStatus OnFailure = TaskStatus.Failure;

        private GameItemInventory _inventory;

        public override void OnStart()
        {
            if (_inventory == null)
            {
                _inventory = GetComponent<GameItemInventory>();
            }
            
            if (_inventory == null) Debug.LogError("No GameItemInventory component found on " + gameObject.name + "!");
        }

        public override TaskStatus OnUpdate()
        {
            if (_inventory == null || target.Value == null) return TaskStatus.Failure;

            return
                _inventory.TryGetItemOfType<AbstractProjectileData>(out _)
                    ? TaskStatus.Success
                    : OnFailure;
        }
    }
}