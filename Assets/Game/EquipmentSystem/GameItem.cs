using System;
using Game.DialogueSystem;
using Game.InteractionSystem;
using Game.Src.EventBusModule;

namespace Game.EquipmentSystem
{
    public class GameItem : InteractableObject
    {
        public GameItemType ItemType;
        public string PickUpDialogue = "";

        private void Start()
        {
            itemName = ItemType.ItemName;
            interactionText = "to pick up";
        }

        public override void Interact(dynamic input)
        {
            if (PickUpDialogue.Length > 0)
            {
                SceneEventBus.Emit(new DialogueEvent(new DialogueItem("", PickUpDialogue)));
            }
        }
    }
}