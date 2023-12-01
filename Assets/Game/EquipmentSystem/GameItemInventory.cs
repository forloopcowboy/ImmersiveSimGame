using System;
using System.Collections.Generic;
using System.Linq;
using Game.InteractionSystem;
using Game.SaveUtility;
using JetBrains.Annotations;
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
        public int[] EquippedItems; // 10 slots for equipped items
        public GameItemInInventory ActivelyHeldItem => ItemsInInventory.FirstOrDefault(i => i.Item.Identifier == ActiveItemId);
        public GameItemInInventory HighlightedItem => ItemsInInventory.FirstOrDefault(i => i.Item.Identifier == HighlightedItemId);
        
        public string ActiveItemId = null;
        public string HighlightedItemId = null;

        [CanBeNull, Required("Interactor is null. Initialize it in the inspector or assign it in code. No interactor will prevent item pickup.", InfoMessageType.Info)] public Interactor interactor;

        private void Start()
        {
            if (!interactor) interactor = GetComponentInChildren<Interactor>();

            if (EquippedItems == null || EquippedItems.Length != 10)
            {
                EquippedItems = new int[10]; // 10 slots for equipped items
                for (var i = 0; i < EquippedItems.Length; i++)
                {
                    EquippedItems[i] = -1;
                }
            }
        }

        public bool TryToPickUpItem(out GameItem item)
        {
            item = null;
            
            var found = interactor != null && interactor.TryToInteract<GameItem, bool>(out item);
            if (found)
            {
                AddItemToInventory(item);
            }

            return found;
        }
        
        public void EquipItem(GameItemInInventory item, int index)
        {
            if (item.Item is EquipableItemType)
            {
                EquippedItems[index] = ItemsInInventory.IndexOf(item);
            }
            else Debug.LogWarning("Item is not equipable.");
        }

        public void HoldItem(GameItemInInventory item)
        {
            ActiveItemId = item.Item.Identifier;
        }

        public void AddItemToInventory(GameItem item)
        {
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.Item.GetInstanceID() == item.ItemType.GetInstanceID());
            if (itemInInventory == null)
            {
                ItemsInInventory.Add(new GameItemInInventory {Item = item.ItemType, Quantity = Mathf.Clamp(item.Amount, 1, 999), Inventory = this});
            }
            else
            {
                itemInInventory.Quantity += Mathf.Clamp(item.Amount, 1, 999);
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
            var currentlyHeldItem = ActivelyHeldItem;
            if (currentlyHeldItem != null && currentlyHeldItem.Item is UsableItemType equipableItem && currentlyHeldItem.Quantity > 0)
            {
                equipableItem.Use(this);
                equipableItem.EmitUseMessage();
            }
        }

        public bool TryGetItemOfType<TItemType>(out TItemType itemType) where TItemType : GameItemType
        {
            itemType = null;
            
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.Item is TItemType);
            if (itemInInventory != null)
            {
                itemType = itemInInventory.Item as TItemType;
                return true;
            }

            return false;
        }

        public SerializedItemData[] GetSerializedInventory()
        {
            return ItemsInInventory.Select(i => new SerializedItemData(i.Item.Identifier, i.Quantity)).ToArray();
        }
    }

    [Serializable]
    public class GameItemInInventory
    {
        public GameItemInventory Inventory;
        public GameItemType Item;
        public int Quantity;
        
        public bool IsHighlighted => Inventory.HighlightedItemId == Item.Identifier;
    }
}