using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Game.AIBehavior
{
    [TaskDescription("Returns success if target exists, and either attack method is not set to ignore OR movement is set to chase.")]
    public class ShouldAimAtTarget : Conditional
    {
        public SharedGameObject target;
        public SharedDamageReactionAttackMethod attackMethod;
        public SharedDamageReactionMovementMethod movementMethod;
        
        public TaskStatus OnFailure = TaskStatus.Failure;

        public override TaskStatus OnUpdate()
        {
            if (target.Value == null) return TaskStatus.Failure;

            return attackMethod.Value == DamageReactionAttackMethod.ATTACK ||
                   movementMethod.Value == DamageReactionMovementMethod.CHASE
                ? TaskStatus.Success
                : OnFailure;
        }
    }
}