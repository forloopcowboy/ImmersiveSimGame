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
        public SharedVector3 offset = new Vector3(0f, 1.5f, 0f);
        public SharedFloat projectileSpeed;
        public SharedFloat projectileHighArcThreshold = 20f;
        public SharedBallisticTrajectory defaultBallisticTrajectory = BallisticTrajectory.Min;
        
        private GameItemInventory _inventory;
        private AimComponent _aimComponent;
        
        public override void OnStart()
        {
            if (_inventory == null)
            {
                _inventory = GetComponent<GameItemInventory>();
            }
            if (_inventory == null) Debug.LogError("No GameItemInventory component found on " + gameObject.name + "!");
            
            if (_aimComponent == null)
            {
                _aimComponent = GetComponent<AimComponent>();
            }
            if (_aimComponent == null) Debug.LogError("No AimComponent found on " + gameObject.name + "!");
            
        }

        public override TaskStatus OnUpdate()
        {
            if (_inventory == null || target.Value == null) return TaskStatus.Failure;

            
            if (_inventory.TryGetItemOfType<AbstractProjectileData>(out var projectileItem))
            {
                if (projectileItem is BallisticProjectileData ballisticProjectile)
                {
                    var strategy = Vector3.Distance(_aimComponent.elevate.position, target.Value.transform.position) > projectileHighArcThreshold.Value
                        ? BallisticTrajectory.LowEnergy
                        : defaultBallisticTrajectory.Value;
                    
                    projectileSpeed.Value = ballisticProjectile.launchSpeed;
                    
                    var velocity = ProjectileSpawner.CalculateBallisticVelocity(_aimComponent.elevate, target.Value.transform.position + offset.Value, ballisticProjectile.launchSpeed, strategy);
                    
                    ballisticProjectile.LaunchProjectile(_inventory, _aimComponent.elevate, velocity);
                    ballisticProjectile.HandleConsumption(_inventory);
                }
                else projectileItem.Use(_inventory);
                
                Debug.Log($"{gameObject.name} targeting {target.Value.name} using {projectileItem.ItemName}");
                
                return TaskStatus.Success;
            }
            
            return TaskStatus.Failure;
        }
    }
}