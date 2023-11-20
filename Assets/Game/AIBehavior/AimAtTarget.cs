using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.ProjectileSystem;

namespace Game.AIBehavior
{
    [TaskDescription("Aims at target until interrupted. Fails if no target is set or no aim component is attached.")]
    public class AimAtTarget : Action
    {
        public SharedGameObject target;
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
            
            _aimComponent.AimAt(target.Value.transform.position, projectileSpeed.Value);
            
            return TaskStatus.Running;
        }
    }
}