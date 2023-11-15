using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.HealthSystem
{
    public class Health : MonoBehaviour
    {
        [Required]
        public HealthSettings settings;

        public float currentHealth;
        public bool isDead;
        
        public UnityEvent onDeath;

        private void Start()
        {
            currentHealth = settings.currentHealth;
            isDead = settings.isDead;
        }

        public void Heal(float amount)
        {
            if (isDead)
                return;
            
            currentHealth += amount;
            if (currentHealth > settings.maxHealth)
                currentHealth = settings.maxHealth;
            
            Debug.Log($"{name} healed for {amount}! Current health: {currentHealth}");
        }
        
        public void TakeDamage(float damage)
        {
            if (settings.isInvincible || isDead)
                return;
            
            currentHealth -= damage;
            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                currentHealth = 0;
                onDeath?.Invoke();

                Debug.Log(name + "is dead!");
            }
            else Debug.Log($"{name} took {damage} damage! Current health: {currentHealth}");
        }
        
    }
}