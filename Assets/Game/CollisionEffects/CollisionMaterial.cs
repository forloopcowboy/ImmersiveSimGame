using System;
using UnityEngine;

namespace Game.CollisionEffects
{
    [Flags] public enum CollisionMaterialType
    {
        Flesh = 1,
        Ice = 2,
        Fire = 4,
        Concrete = 8,
        Default = 16,
        Any = ~0
    }
    
    public class CollisionMaterial : MonoBehaviour
    {
        public CollisionMaterialType Type;
    }
}