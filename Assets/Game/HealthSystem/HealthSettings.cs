using UnityEngine;

namespace Game.HealthSystem
{
    [CreateAssetMenu(fileName = "Untitled Health Settings", menuName = "Health/New HealthSettings", order = 0)]
    public class HealthSettings : ScriptableObject
    {
        public float maxHealth = 100f;
        public float currentHealth = 100f;
        public float healthRegenRate = 0f;
        public float healthRegenDelay = 0f;
        public float healthRegenAmount = 0f;
        public float healthRegenInterval = 0f;
        public bool isInvincible = false;
        public bool isDead = false;
        public bool isRegenerating = false;
        public MaterialType materialType = MaterialType.Default;
    }
}