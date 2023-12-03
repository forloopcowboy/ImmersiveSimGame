using BehaviorDesigner.Runtime;
using Game.ProjectileSystem;

namespace Game.AIBehavior
{
    public class SharedBallisticTrajectory : SharedVariable<BallisticTrajectory>
    {
        public override string ToString() { return mValue.ToString(); }
        
        public static implicit operator SharedBallisticTrajectory(BallisticTrajectory value) { return new SharedBallisticTrajectory { mValue = value }; }
    }
}