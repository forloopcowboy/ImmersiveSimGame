using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.HealthSystem
{
    public enum DamageMethod
    {
        Fixed,
        FullHealth,
        Percentage,
        RandomBetweenRange
    }
    
    [CreateAssetMenu(fileName = "Untitled DamageSettings", menuName = "Health/New DamageSettings", order = 0)]
    public class DamageSettings : SerializedScriptableObject
    {
        public MaterialType damageMaterialType = MaterialType.Default;
        public DamageMethod method = DamageMethod.Fixed;
        
        [ShowIf("IsFixed")]
        public float fixedDamage = 10f;
        [Range(0f, 1f), ShowIf("IsPercentage")]
        public float percentageDamage = .10f;
        [ShowIf("IsRandomBetweenRange")]
        public Vector2 randomDamageRange = Vector2.up;

        public float cooldownInSeconds;
        
        public bool IsFixed => method == DamageMethod.Fixed;
        public bool IsInstantKill => method == DamageMethod.FullHealth;
        public bool IsPercentage => method == DamageMethod.Percentage;
        public bool IsRandomBetweenRange => method == DamageMethod.RandomBetweenRange;

        public float GetValue(float currentHealth)
        {
            switch (method)
            {
                case DamageMethod.Fixed:
                    return fixedDamage;
                case DamageMethod.FullHealth:
                    return Single.MaxValue;
                case DamageMethod.Percentage:
                    return percentageDamage * currentHealth;
                case DamageMethod.RandomBetweenRange:
                    return UnityEngine.Random.Range(
                        Mathf.Min(randomDamageRange.x, randomDamageRange.y), 
                        Mathf.Max(randomDamageRange.x, randomDamageRange.y)
                    );
                default:
                    throw new Exception($"Invalid damage method {method}");
            }
        }
    }
}