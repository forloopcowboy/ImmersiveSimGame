using System;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.EquipmentSystem
{
    /// <summary>
    /// If quantity is zero, this item will destroy itself.
    /// </summary>
    public class GameItemThumbnailUIController : MonoBehaviour
    {
        public GameItemInInventory GameItemInInventory;
        public TMP_Text quantityText;
        public TMP_Text equipIndicatorText;
        public Image itemThumbnailImage;
        public Button button;

        public void OnEnable()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                throw new NullReferenceException("Button is null. Initialize it in the inspector or assign it in code.");
            }
            
            button.Select();
        }

        private void Update()
        {
            if (GameItemInInventory == null || GameItemInInventory.Item == null || GameItemInInventory.Quantity <= 0)
            {
                quantityText.transform.parent.gameObject.SetActive(false);
                itemThumbnailImage.enabled = false;
            }
            else
            {
                itemThumbnailImage.enabled = true;
                quantityText.text = GameItemInInventory.Quantity.ToString();
                GameItemInInventory.Item.GenerateIconSprite();
                itemThumbnailImage.sprite = GameItemInInventory.Item.ItemSprite;
            }

            if (GameItemInInventory != null && GameItemInInventory.Quantity <= 0)
            {
                GameItemInInventory = null;
            }
            else if (GameItemInInventory?.Quantity > 1)
            {
                quantityText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                quantityText.transform.parent.gameObject.SetActive(false);
            }
        }
        
        public void OnClick()
        {
            if (GameItemInInventory.Item is EquipableItemType equipableItemType)
                equipableItemType.Equip(GameItemInInventory.Inventory);
            else if (GameItemInInventory.Item is UsableItemType usableItemType)
            {
                usableItemType.Use(GameItemInInventory.Inventory);
                usableItemType.EmitUseMessage();
            }
            else
            {
                Debug.Log($"{GameItemInInventory.Item.ItemName} is not usable.");
            }
        }
    }
}