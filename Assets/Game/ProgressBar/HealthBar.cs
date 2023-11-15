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
        
        public void SetValues(Health fromHealthComponent)
        {
            // lerp
            current = Mathf.Lerp(current, fromHealthComponent.currentHealth, Time.deltaTime * 10f);
            max = fromHealthComponent.settings.maxHealth;
        }
        
        private void Update()
        {
            SetValues(health);
            UpdateFill();
        }
    }
}