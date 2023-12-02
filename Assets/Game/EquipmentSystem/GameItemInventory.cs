using System;
using System.Collections.Generic;
using System.Linq;
using Game.InteractionSystem;
using Game.SaveUtility;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
        public GameItemInInventory ActivelyHeldItem => ItemsInInventory.FirstOrDefault(i => i.ItemType.Identifier == ActiveItemId);
        public GameItemInInventory HighlightedItem => ItemsInInventory.FirstOrDefault(i => i.ItemType.Identifier == HighlightedItemId);
        
        public string ActiveItemId = null;
        public string HighlightedItemId = null;

        [CanBeNull, Required("Interactor is null. Initialize it in the inspector or assign it in code. No interactor will prevent item pickup.", InfoMessageType.Info)] 
        public Interactor interactor;

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

        public bool TryToPickUpItem(out GameItemInteractable itemInteractable)
        {
            itemInteractable = null;
            var found = interactor != null && interactor.TryToInteract<GameItemInteractable, Action<GameItemInteractable>>(out itemInteractable, AddItemToInventory);
            return found;
        }
        
        public void EquipItem(GameItemInInventory item, int equipAtIndex)
        {
            if (item.ItemType is EquipableItemType)
            {
                var itemIndex = ItemsInInventory.IndexOf(item);
                
                // Clear out all slots with this index, to avoid duplicate equipped items
                for (var i = 0; i < EquippedItems.Length; i++)
                {
                    if (EquippedItems[i] == itemIndex)
                    {
                        EquippedItems[i] = -1;
                    }
                }
                
                // Equoip item
                EquippedItems[equipAtIndex] = itemIndex;
            }
            else Debug.LogWarning("Item is not equipable.");
        }

        public void HoldItem(GameItemInInventory item)
        {
            ActiveItemId = item.ItemType.Identifier;
        }

        public void AddItemToInventory(GameItemInteractable itemInteractable)
        {
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.ItemType.GetInstanceID() == itemInteractable.ItemType.GetInstanceID());
            if (itemInInventory == null)
            {
                ItemsInInventory.Add(new GameItemInInventory {ItemType = itemInteractable.ItemType, Quantity = Mathf.Clamp(itemInteractable.Amount, 1, 999), Inventory = this});
            }
            else
            {
                itemInInventory.Quantity += Mathf.Clamp(itemInteractable.Amount, 1, 999);
            }
        }

        public void ConsumeItem(GameItemType type, int quantity = 1)
        {
            if (quantity < 1) throw new Exception("Quantity must be at least 1.");
            
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.ItemType.GetInstanceID() == type.GetInstanceID());
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
            if (currentlyHeldItem != null && currentlyHeldItem.ItemType is UsableItemType equipableItem && currentlyHeldItem.Quantity > 0)
            {
                equipableItem.Use(this);
                equipableItem.EmitUseMessage();
            }
        }

        public bool TryGetItemOfType<TItemType>(out TItemType itemType) where TItemType : GameItemType
        {
            itemType = null;
            
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.ItemType is TItemType);
            if (itemInInventory != null)
            {
                itemType = itemInInventory.ItemType as TItemType;
                return true;
            }

            return false;
        }

        public SerializedItemData[] GetSerializedInventory()
        {
            return ItemsInInventory.Select(i => new SerializedItemData(i.ItemType.Identifier, i.Quantity)).ToArray();
        }
    }

    [Serializable]
    public class GameItemInInventory
    {
        public GameItemInventory Inventory;
        [FormerlySerializedAs("Item")] public GameItemType ItemType;
        public int Quantity;
        
        public bool IsHighlighted => Inventory.HighlightedItemId == ItemType.Identifier;

        public void Highlight()
        {
            Inventory.HighlightedItemId = ItemType.Identifier;
        }
    }
}