using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    // TODO: Inherit this to make a heal potion
    // TODO: implement health component (copy-paste)
    public abstract class UsableItemType : GameItemType
    {
        [InfoBox("The message to display when the item is used. Use $itemName to display the item name.")]
        public string useMessage = "used $itemName";
        
        public abstract void Use(GameItemInventory user);
    }
}