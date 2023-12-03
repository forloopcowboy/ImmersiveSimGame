namespace Game.AIBehavior
{
    public enum DamageReactionMovementMethod
    {
        FLEE,
        CHASE,
        STAND_GROUND
    }
    
    public enum DamageReactionAttackMethod
    {
        ATTACK,
        IGNORE
    }

    public enum AimingMethod
    {
        /// <summary>
        /// Attack using distance threshold when visible.
        /// </summary>
        AttackWhenVisible,
        
        /// <summary>
        /// Attack using distance threshold when visible,
        /// and when target not visible attacks using ballistic aiming.
        /// </summary>
        BallisticWhenInvisible
    }
}