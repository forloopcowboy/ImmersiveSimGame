using BehaviorDesigner.Runtime.Tasks;
using Game.ProjectileSystem;

namespace Game.AIBehavior
{
    public class SetSharedBallisticTrajectoryMethod : Action
    {
        public BallisticTrajectory valueToSet;
        public SharedBallisticTrajectory mSharedBallisticTrajectory;
        
        public override TaskStatus OnUpdate()
        {
            mSharedBallisticTrajectory.SetValue(valueToSet);
            return TaskStatus.Success;
        }
    }
}