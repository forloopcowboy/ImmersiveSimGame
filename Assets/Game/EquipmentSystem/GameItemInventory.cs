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
        public Interactor interactor;

        private void Start()
        {
            if (!interactor) interactor = GetComponentInChildren<Interactor>();
            if (!interactor) throw new NullReferenceException("Interactor is null. Initialize it in the inspector or assign it in code.");
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

        private void AddItemToInventory(GameItem item)
        {
            var itemInInventory = ItemsInInventory.FirstOrDefault(i => i.Item.GetInstanceID() == item.ItemType.GetInstanceID());
            if (itemInInventory == null)
            {
                ItemsInInventory.Add(new GameItemInInventory {Item = item.ItemType, Quantity = 1});
            }
            else
            {
                itemInInventory.Quantity++;
            }
            
            Destroy(item.gameObject);
        }
    }

    [Serializable]
    public class GameItemInInventory
    {
        public GameItemType Item;
        public int Quantity;
    }
}