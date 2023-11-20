using Game.EquipmentSystem;
using UnityEngine;

namespace Game.HealthSystem
{
    [CreateAssetMenu(fileName = "New Health Potion", menuName = "GameItem/NewHealthPotion", order = 0)]
    public class HealthPotionItem : UsableItemType
    {
        public DamageSettings healSettings;
        
        public override void Use(GameItemInventory user)
        {
            base.Use(user);
            var health = user.GetComponent<Health>();
            
            health.Heal(healSettings.GetValue(health.currentHealth), user.gameObject);
        }
    }
}