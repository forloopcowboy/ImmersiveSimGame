using System;
using Game.DialogueSystem;
using Game.InteractionSystem;
using Game.SaveUtility;
using Game.Src.EventBusModule;
using Game.Src.IconGeneration;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    public class GameItem : InteractableObject
    {
        [TabGroup("GameItem"), ReadOnly] public string ItemId = Guid.NewGuid().ToString();
        [TabGroup("GameItem")] public GameItemType ItemType;
        [TabGroup("GameItem")] public string PickUpDialogue = "";
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
            GlobalGameState.State.CurrentLevelState.SetItemPickedUp(ItemId);
            
            if (PickUpDialogue.Length > 0)
            {
                SceneEventBus.Emit(new DialogueEvent(new DialogueItem("", PickUpDialogue.Replace("$itemName", itemName).Replace("$ItemName", itemName))));
            }
        }
    }
}