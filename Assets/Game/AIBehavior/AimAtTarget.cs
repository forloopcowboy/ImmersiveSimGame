using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.ProjectileSystem;
using UnityEngine;

namespace Game.AIBehavior
{
    [TaskDescription("Aims at target until interrupted. Fails if no target is set or no aim component is attached.")]
    public class AimAtTarget : Action
    {
        public SharedGameObject target;
        public SharedVector3 offset = new Vector3(0f, 1.5f, 0f);
        public SharedFloat projectileSpeed;
        public SharedFloat projectileHighArcThreshold = 20f;
        
        private AimComponent _aimComponent;

        public override void OnStart()
        {
            _aimComponent = GetComponent<AimComponent>();
            
            if (_aimComponent == null)
            {
                UnityEngine.Debug.LogError("No AimComponent found on " + gameObject.name + "!");
            }
        }
        
        public override TaskStatus OnUpdate()
        {
            if (_aimComponent == null || target.Value == null) return TaskStatus.Failure;
            
            var strategy = Vector3.Distance(_aimComponent.elevate.position, target.Value.transform.position) > projectileHighArcThreshold.Value
                ? BallisticTrajectory.LowEnergy
                : BallisticTrajectory.Min;
            
            if (projectileSpeed.Value <= 0f) projectileSpeed.Value = 1f;
            
            _aimComponent.AimAt(target.Value.transform.position + offset.Value, projectileSpeed.Value, strategy);
            
            return TaskStatus.Running;
        }
    }
}