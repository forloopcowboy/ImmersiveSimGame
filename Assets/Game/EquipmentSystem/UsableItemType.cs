using Game.InteractionSystem;
using Game.Src.EventBusModule;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    public abstract class UsableItemType : GameItemType
    {
        [InfoBox("The message to display when the item is used. Use $itemName to display the item name.")]
        public string useMessage = "used $itemName";

        [Tooltip("If true, the item quantity in inventory will be decreased.")]
        public bool consumable = true;

        public virtual void Use(GameItemInventory user)
        {
            HandleConsumption(user);
        }
        
        public void HandleConsumption(GameItemInventory user)
        {
            if (consumable)
            {
                user.ConsumeItem(this);
            }
        }
        
        public void EmitUseMessage()
        {
            SceneEventBus.Emit(new NotificationEvent(useMessage.Replace("$itemName", ItemName)));
        }
    }
}