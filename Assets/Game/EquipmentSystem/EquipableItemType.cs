using System.Linq;
using Game.Src.EventBusModule;
using Game.Utils;
using UnityEngine;

namespace Game.EquipmentSystem
{
    // TODO: Make this abstract
    [CreateAssetMenu(fileName = "Equipable Item", menuName = "GameItem/New EquipableItemType", order = 0)]
    public abstract class EquipableItemType : UsableItemType
    {
        public void Equip(GameItemInventory user)
        {
            var gameItemInInventory = user.ItemsInInventory.First(i => i.ItemType.GetInstanceID() == GetInstanceID());
            if (gameItemInInventory == null) throw new System.Exception("Item not found in inventory. Can only equip items from inventory.");
            
            SceneEventBus.Emit(new TryEquipItemEvent(gameItemInInventory));
            SceneEventBus.Emit(new NotificationEvent($"Press number 1...9 to equip {gameItemInInventory.ItemType.ItemName}."));
            
            user.HoldItem(gameItemInInventory);
        }
    }
}