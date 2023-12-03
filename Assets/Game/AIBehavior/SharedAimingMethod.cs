using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Game.AIBehavior
{
    public class SharedAimingMethod : SharedVariable<AimingMethod>
    {
        public override string ToString()
        {
            return mValue.ToString();
        }

        public static implicit operator SharedAimingMethod(AimingMethod value) { return new SharedAimingMethod { mValue = value }; }
    }
    
    public class CompareSharedAimingMethod : Conditional
    {
        public SharedAimingMethod mSharedAimingMethod;
        public AimingMethod mValueToCompare;
        
        public override TaskStatus OnUpdate()
        {
            return mSharedAimingMethod.Value == mValueToCompare ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}