using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.HealthSystem
{
    public class DamageComponent : MonoBehaviour
    {
        [InfoBox("This component is used to damage other objects. It requires a Health component on objects it interacts with for it to work. You may call the interaction function from an DoOnCollision component.")]
        [Required, InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public DamageSettings settings;
        
        public DateTime lastDamageTime = DateTime.MinValue;

        public float GetDamageValue(float currentHealthValue)
        {
            if (settings.cooldownInSeconds > 0)
            {
                var timeSinceLastDamage = DateTime.Now - lastDamageTime;
                if (timeSinceLastDamage.TotalSeconds < settings.cooldownInSeconds)
                    return 0;
            }

            lastDamageTime = DateTime.Now;
            return settings.GetValue(currentHealthValue);
        }
        
        /// <summary>
        /// Applies damage to the object of a collision. Used to inflict damage on others.
        /// </summary>
        public void DamageOnCollision(Collision collision)
        {
            var health = collision.gameObject.GetComponent<Health>();
            if (!health)
                return;
            
            if (MaterialInteractions.CanDamage(settings.damageMaterialType, health.settings.materialType))
                health.TakeDamage(GetDamageValue(health.settings.currentHealth));
        }
        
        /// <summary>
        /// Applies damage to the object of a collision. Used to inflict damage on others.
        /// </summary>
        public void DamageOnTrigger(Collider c)
        {
            var health = c.gameObject.GetComponent<Health>() ?? c.gameObject.GetComponentInParent<Health>();
            if (!health)
                return;
            
            if (MaterialInteractions.CanDamage(settings.damageMaterialType, health.settings.materialType))
                health.TakeDamage(GetDamageValue(health.settings.currentHealth));
        }
    }
}