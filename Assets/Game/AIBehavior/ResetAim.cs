using BehaviorDesigner.Runtime.Tasks;
using Game.ProjectileSystem;

namespace Game.AIBehavior
{
    [TaskDescription("Resets aim gradually until interrupted. Fails if no aim component is attached.")]
    public class ResetAim : Action
    {
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
            if (_aimComponent == null) return TaskStatus.Failure;
            _aimComponent.ResetAim();
            
            return TaskStatus.Running;
        }
    }
}