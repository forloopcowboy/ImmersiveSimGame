using System;
using Game.HealthSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.ProgressBar
{
    [ExecuteAlways]
    public class HealthBar : ProgressBar
    {
        [Required]
        public Health health;

        public void SetValues(Health fromHealthComponent, bool lerp = true)
        {
            // lerp
            current = lerp ? Mathf.Lerp(current, fromHealthComponent.currentHealth, Time.deltaTime * 10f) : fromHealthComponent.currentHealth;
            max = fromHealthComponent.settings.maxHealth;
        }

        private void Update()
        {
            SetValues(health);
            UpdateFill();
        }
    }
}