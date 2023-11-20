using BehaviorDesigner.Runtime.Tasks;

namespace Game.AIBehavior
{
    public class CompareReactionMovementMethod : Conditional
    {
        public SharedDamageReactionMovementMethod Variable;
        public DamageReactionMovementMethod Value;

        public override TaskStatus OnUpdate()
        {
            return Variable.Value == Value ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            Variable = DamageReactionMovementMethod.STAND_GROUND;
            Value = DamageReactionMovementMethod.STAND_GROUND;
        }
    }
}