using System;
using Game.Src.EventBusModule;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    public abstract class UsableItemType : GameItemType
    {
        [TabGroup("Usage"), InfoBox("The message to display when the item is used. Use $itemName to display the item name.")]
        public string useMessage = "used $itemName";

        [TabGroup("Usage"), Tooltip("If true, the item quantity in inventory will be decreased.")]
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
            if (!String.IsNullOrEmpty(useMessage)) SceneEventBus.Emit(new NotificationEvent(useMessage.Replace("$itemName", ItemName)));
        }
    }
}