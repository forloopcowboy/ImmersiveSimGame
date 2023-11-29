using System;
using Game.DialogueSystem;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
using Game.Src.IconGeneration;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    public class GameItem : InteractableObject
    {
        [TabGroup("GameItem")] public GameItemType ItemType;
        [TabGroup("GameItem")] public string PickUpDialogue = "";
        [TabGroup("GameItem")] public int Amount = 1;
        [TabGroup("GameItem")] public bool UseItemTypeName = true;

        private void Start()
        {
            if (ItemType != null && UseItemTypeName) itemName = ItemType.ItemName;
            interactionText = "to pick up";
            
            if (ItemType != null && ItemType.IconPrefab != null && !ItemType.IconSpriteGenerated) IconPrinter.Singleton.GetIcon(ItemType.IconPrefab, icon =>
            {
                if (!ItemType.IconSpriteGenerated)
                    ItemType.ItemSprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
                ItemType.IconSpriteGenerated = true;
                Debug.Log("Created item sprite.");
            });
        }

        protected override void OnInteract(Interactor interactor, dynamic input)
        {
            if (PickUpDialogue.Length > 0)
            {
                SceneEventBus.Emit(new DialogueEvent(new DialogueItem("", PickUpDialogue.Replace("$itemName", itemName))));
            }
        }
    }
}