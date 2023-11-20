using BehaviorDesigner.Runtime.Tasks;

namespace Game.AIBehavior
{
    public class CompareReactionAttackMethod : Conditional
    {
        public SharedDamageReactionAttackMethod Variable;
        public DamageReactionAttackMethod Value;

        public override TaskStatus OnUpdate()
        {
            return Variable.Value == Value ? TaskStatus.Success : TaskStatus.Failure;
        }

        public override void OnReset()
        {
            Variable = DamageReactionAttackMethod.IGNORE;
            Value = DamageReactionAttackMethod.IGNORE;
        }
    }
}