using System;
using System.Collections.Generic;
using System.Linq;
using Game.InteractionSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    /// <summary>
    /// To be attached to game objects that can pick up items.
    /// Expects a collider trigger to be attached to the same game object.
    /// Queues the item for pickup when the player enters the trigger.
    /// Pick up occurs by order of trigger enter - Last in first out.
    /// </summary>
    public class GameItemInventory : SerializedMonoBehaviour
    {
        [SerializeField]
        public List<GameItemInInventory> ItemsInInventory = new();
        public GameItemInInventory[] EquippedItems = new GameItemInInventory[10]; // 10 slots for equipped items
        public GameItemInInventory activelyHeldItem = null;
        
        public Interactor interactor;

        private void Start()
        {
            if (!interactor) interactor = GetComponentInChildren<Interactor>();
            if (!interactor) throw new NullReferenceException("Interactor is null. Initialize it in the inspector or assign it in code.");
            
            EquippedItems = new GameItemInInventory[10]; // 10 slots for equipped items
        }

        public bool TryToPickUpItem(out GameItem item)
        {
            var found = interactor.TryToInteract<GameItem, bool>(out item);
            if (found)
            {
                AddItemToInventory(item);
            }

            return found;
        }
        
        public void EquipItem(GameItemInInventory item, int index)
        {
            if (item.Item is EquipableItemType equipableItem)
            {
                EquippedItems[index] = item;
            }
            else Debug.LogWarning("Item is not equipable.");
        }

        public void HoldItem(GameItemInInventory item)
        {
            activelyHeldItem = item;
        }

        private void AddItemToInventory(GameItem item)
        {
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.Item.GetInstanceID() == item.ItemType.GetInstanceID());
            if (itemInInventory == null)
            {
                ItemsInInventory.Add(new GameItemInInventory {Item = item.ItemType, Quantity = 1, Inventory = this});
            }
            else
            {
                itemInInventory.Quantity++;
            }
            
            Destroy(item.gameObject);
        }

        public void ConsumeItem(GameItemType type, int quantity = 1)
        {
            if (quantity < 1) throw new Exception("Quantity must be at least 1.");
            
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.Item.GetInstanceID() == type.GetInstanceID());
            if (itemInInventory != null)
            {
                itemInInventory.Quantity -= quantity;
                if (itemInInventory.Quantity <= 0)
                    ItemsInInventory.Remove(itemInInventory);
            }
            
        }

        public void UseItemInHand()
        {
            var currentlyHeldItem = activelyHeldItem;
            if (currentlyHeldItem != null && currentlyHeldItem.Item is UsableItemType equipableItem && currentlyHeldItem.Quantity > 0)
            {
                equipableItem.Use(this);
                equipableItem.EmitUseMessage();
            }
        }
    }

    [Serializable]
    public class GameItemInInventory
    {
        public GameItemInventory Inventory;
        public GameItemType Item;
        public int Quantity;
    }
}