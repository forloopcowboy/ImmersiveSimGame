using System;
using Game.InteractionSystem;

namespace Game.EquipmentSystem
{
    public class GameItem : InteractableObject
    {
        public GameItemType ItemType;

        private void Start()
        {
            itemName = ItemType.ItemName;
            interactionText = "to pick up";
        }
    }
}