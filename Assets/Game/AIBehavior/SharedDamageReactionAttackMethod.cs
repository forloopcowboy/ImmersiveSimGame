using BehaviorDesigner.Runtime;

namespace Game.AIBehavior
{
	[System.Serializable]
	public class SharedDamageReactionAttackMethod : SharedVariable<DamageReactionAttackMethod>
	{
		public override string ToString() { return mValue.ToString(); }
		public static implicit operator SharedDamageReactionAttackMethod(DamageReactionAttackMethod value) { return new SharedDamageReactionAttackMethod { mValue = value }; }
	}
}