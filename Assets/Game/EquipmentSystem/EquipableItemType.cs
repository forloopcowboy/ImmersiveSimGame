using UnityEngine;

namespace Game.EquipmentSystem
{
    [CreateAssetMenu(fileName = "Equipable Item", menuName = "GameItem/New EquipableItemType", order = 0)]
    public class EquipableItemType : UsableItemType
    {
        public override void Use(GameItemInventory user)
        {
            Debug.Log("Equipable items cannot be used yet.");
        }
    }
}