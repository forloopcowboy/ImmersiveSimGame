using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    [CreateAssetMenu(fileName = "Usable Item Type", menuName = "GameItem/New UsableItemType", order = 0)]
    public abstract class UsableItemType : GameItemType
    {
        [InfoBox("The message to display when the item is used. Use $itemName to display the item name.")]
        public string useMessage = "used $itemName";
        
        public abstract void Use(GameObject user);
    }
}