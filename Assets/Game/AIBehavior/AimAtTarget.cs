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
            
            _aimComponent.AimAt(target.Value.transform.position + offset.Value, projectileSpeed.Value);
            
            return TaskStatus.Running;
        }
    }
}