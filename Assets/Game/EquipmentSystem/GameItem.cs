using System;
using Game.DialogueSystem;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
using Game.Src.IconGeneration;
using UnityEngine;

namespace Game.EquipmentSystem
{
    public class GameItem : InteractableObject
    {
        public GameItemType ItemType;
        public string PickUpDialogue = "";

        private void Start()
        {
            if (ItemType != null) itemName = ItemType.ItemName;
            interactionText = "to pick up";
            
            if (ItemType != null && ItemType.IconPrefab != null && !ItemType.IconSpriteGenerated) IconPrinter.Singleton.GetIcon(ItemType.IconPrefab, icon =>
            {
                if (!ItemType.IconSpriteGenerated)
                    ItemType.ItemSprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
                ItemType.IconSpriteGenerated = true;
                Debug.Log("Created item sprite.");
            });
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