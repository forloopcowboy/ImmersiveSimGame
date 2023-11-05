using UnityEngine;

namespace Game.EquipmentSystem
{
    // TODO: Make this abstract
    [CreateAssetMenu(fileName = "Equipable Item", menuName = "GameItem/New EquipableItemType", order = 0)]
    public class EquipableItemType : UsableItemType
    {
        public override void Use(GameItemInventory user)
        {
            // TODO: Make extension for projectile consumable item
            // Using this item while equipped will fire a projectile
            // make projectile do damage to dummy
            // copy paste code from progress bar to make a health bar
            Debug.Log("Equipable items cannot be used yet.");
        }
    }
}