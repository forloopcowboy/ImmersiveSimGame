using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.HealthSystem;
using UnityEngine;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if the agent has died.")]
    public class HasDied : Conditional
    {
        public SharedGameObject target;
        
        public TaskStatus OnFailure = TaskStatus.Failure;
        private Health health;

        public override void OnStart()
        {
            if (target.Value == null)
                target.Value = gameObject;
            
            health = target.Value.GetComponent<Health>();
        }

        public override TaskStatus OnUpdate()
        {
            return health.isDead ? TaskStatus.Success : OnFailure;
        }
    }
}