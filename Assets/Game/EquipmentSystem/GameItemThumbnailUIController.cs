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
            if (GameItemInInventory == null)
            {
                throw new NullReferenceException("GameItemInInventory is null. Initialize it in the inspector or assign it in code.");
            }
            
            if (GameItemInInventory.Item == null)
            {
                throw new NullReferenceException("GameItemInInventory.GameItem is null. Initialize it in the inspector or assign it in code.");
            }
            
            if (GameItemInInventory.Quantity <= 0)
            {
                Destroy(gameObject);
            }

            if (GameItemInInventory.Quantity > 1)
            {
                quantityText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                quantityText.transform.parent.gameObject.SetActive(false);
            }
            
            quantityText.text = GameItemInInventory.Quantity.ToString();
            itemThumbnailImage.sprite = GameItemInInventory.Item.ItemSprite;
        }
        
        public void OnClick()
        {
            if (GameItemInInventory == null)
            {
                throw new NullReferenceException("GameItemInInventory is null. Initialize it in the inspector or assign it in code.");
            }
            
            if (GameItemInInventory.Item == null)
            {
                throw new NullReferenceException("GameItemInInventory.GameItem is null. Initialize it in the inspector or assign it in code.");
            }
            
            if (GameItemInInventory.Item is UsableItemType usableItem)
            {
                usableItem.Use(GameItemInInventory.Inventory.gameObject);
                GameItemInInventory.Quantity--;
                SceneEventBus.Emit(new NotificationEvent(usableItem.useMessage.Replace("$itemName", GameItemInInventory.Item.ItemName)));
            }
            else
            {
                Debug.Log("Item is not usable.");
            }
        }
    }
}