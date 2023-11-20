using BehaviorDesigner.Runtime.Tasks;
using Game.HealthSystem;
using UnityEngine;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if the agent has died.")]
    public class HasDied : Conditional
    {
        public TaskStatus OnFailure = TaskStatus.Failure;
        private Health health;

        public override void OnStart()
        {
            if (health == null)
            {
                health = GetComponent<Health>();
            }
        }

        public override TaskStatus OnUpdate()
        {
            return health.isDead ? TaskStatus.Success : OnFailure;
        }
    }
}