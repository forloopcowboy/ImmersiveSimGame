using System;

namespace Game.HealthSystem
{
    public enum MaterialType
    {
        Human,
        Fire,
        Ice,
        Default,
    }

    public static class MaterialInteractions
    {
        public static bool CanDamage(MaterialType attacker, MaterialType defender)
        {
            if (attacker == MaterialType.Default && defender == MaterialType.Default)
                return false;
            
            switch (attacker)
            {
                case MaterialType.Human:
                    switch (defender)
                    {
                        case MaterialType.Human:
                            return true;
                        case MaterialType.Fire:
                            return false;
                        case MaterialType.Ice:
                            return true;
                        case MaterialType.Default:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(defender), defender, $"Invalid MaterialType - not implemented yet. {attacker} cannot damage {defender}.");
                    }
                case MaterialType.Fire:
                    return defender != MaterialType.Fire;
                case MaterialType.Ice:
                    return defender != MaterialType.Ice;
                default:
                    return false;
            }
        }
    }
}