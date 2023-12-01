using System;
using TMPro;
using UnityEngine;

namespace Game.EquipmentSystem
{
    public class DetailedItemUIController : MonoBehaviour
    {
        public GameItemInventory Inventory;
        
        public Transform root;
        public TMP_Text itemNameText;
        public TMP_Text itemDescriptionText;
        public TMP_Text itemValueText;
        
        public bool IsOpen => root.gameObject.activeSelf;
        
        public void ToggleDetailedItem(bool? valueToSet = null)
        {
            bool value = valueToSet.HasValue ? valueToSet.Value : !root.gameObject.activeSelf;
            root.gameObject.SetActive(value);
        }
        
        public void SetItem(GameItemInInventory item)
        {
            if (item == null)
            {
                ToggleDetailedItem(false);
                return;
            }
            
            ToggleDetailedItem(true);
            
            if (itemNameText) itemNameText.text = item.Item.ItemName;
            if (itemDescriptionText) itemDescriptionText.text = item.Item.GetItemDescription();
            if (itemValueText) itemValueText.text = item.Item.ItemValue.ToString();
        }

        private void Update()
        {
            SetItem(Inventory.HighlightedItem);
        }
    }
}