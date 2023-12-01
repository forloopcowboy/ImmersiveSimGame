using System;
using Game.DialogueSystem;
using Game.InteractionSystem;
using Game.SaveUtility;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    public class GameItemInteractable : InteractableObject
    {
        [TabGroup("GameItem"), ReadOnly] public string ItemId = Guid.NewGuid().ToString();
        [TabGroup("GameItem")] public GameItemType ItemType;
        [TabGroup("GameItem")] public int Amount = 1;
        [TabGroup("GameItem")] public bool UseItemTypeName = true;

        private void Start()
        {
            // If item already has been picked up, destroy self
            if (GlobalGameState.State.CurrentLevelState.IsItemPickedUp(ItemId))
            {
                Destroy(gameObject);
                return;
            }
            
            if (ItemType != null && UseItemTypeName) itemName = ItemType.ItemName;
            interactionText = "to pick up";
        }

        protected override void OnInteract(Interactor interactor, dynamic input)
        {
            // Adds item to Interactor inventory
            Action<GameItemInteractable> handlePickUp;
            
            if (input is Action<GameItemInteractable> action)
                handlePickUp = action;
            else if (interactor.TryGetComponent(out GameItemInventory inventory))
                handlePickUp = inventory.AddItemToInventory;
            else
            {
                Debug.LogError("Could not interact with GameItemInteractable. No GameItemInventory found on Interactor.");
                return;
            }
            
            handlePickUp(this);
            
            // Save item as picked up
            GlobalGameState.State.CurrentLevelState.SetItemPickedUp(ItemType.Identifier);
            
            // Destroy self
            Destroy(gameObject);
        }
    }
}