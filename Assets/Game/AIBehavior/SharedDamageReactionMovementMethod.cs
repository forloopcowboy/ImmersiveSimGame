using BehaviorDesigner.Runtime;

namespace Game.AIBehavior
{
	[System.Serializable]
	public class SharedDamageReactionMovementMethod : SharedVariable<DamageReactionMovementMethod>
	{
		public override string ToString() { return mValue.ToString(); }
		public static implicit operator SharedDamageReactionMovementMethod(DamageReactionMovementMethod value) { return new SharedDamageReactionMovementMethod { mValue = value }; }
	}
}