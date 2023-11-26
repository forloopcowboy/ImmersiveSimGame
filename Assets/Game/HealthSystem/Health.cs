using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.HealthSystem
{
    public class Health : SerializedMonoBehaviour
    {
        [Required]
        public HealthSettings settings;

        public float currentHealth;
        public bool isDead;
        
        [InfoBox("Called when the object dies. Argument is damage source")]
        public UnityEvent<GameObject> onDeath;
        [InfoBox("Called when the object takes damage, even if it dies. Argument is damage source")]
        public UnityEvent<GameObject> onDamage;
        [InfoBox("Called when the object is healed. Argument is damage source")]
        public UnityEvent<GameObject> onHeal;
        
        [Button]
        private void SynchronizeHealth()
        {
            currentHealth = settings.currentHealth;
            isDead = settings.isDead;
        }

        private void Start()
        { 
            SynchronizeHealth();
        }

        public void Heal(float amount, GameObject source)
        {
            if (isDead)
                return;
            
            currentHealth += amount;
            if (currentHealth > settings.maxHealth)
                currentHealth = settings.maxHealth;
            
            var healSrcMsg = source != null ? $" by {source.name}" : "";
            Debug.Log($"{name} healed for {amount}{healSrcMsg}! Current health: {currentHealth}");
            
            onHeal?.Invoke(source);
        }
        
        public void TakeDamage(float damage, GameObject source)
        {
            if (settings.isInvincible || isDead)
                return;
            
            var dmgSrcMsg = source != null ? $" from {source.name}" : "";
            currentHealth -= damage;
            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                currentHealth = 0;
                onDeath?.Invoke(source);

                Debug.Log(name + "is dead!");
            }
            // else Debug.Log($"{name} took {damage} damage{dmgSrcMsg}! Current health: {currentHealth}");
            
            onDamage?.Invoke(source);
        }
        
    }
}