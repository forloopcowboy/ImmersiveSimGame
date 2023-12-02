using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.EquipmentSystem
{
    /// <summary>
    /// Shows high level information about a game item.
    /// Includes logic for highlighting the item when the mouse is over it.
    /// </summary>
    public class GameItemThumbnailUIController : MonoBehaviour, IPointerEnterHandler
    {
        public GameItemInInventory GameItemInInventory;
        public TMP_Text quantityText;
        public TMP_Text equipIndicatorText;
        public Image itemThumbnailImage;
        public Button button;

        public void OnEnable()
        {
            if (button == null) button = GetComponent<Button>();
            if (button == null)
            {
                throw new NullReferenceException("Button is null. Initialize it in the inspector or assign it in code.");
            }
        }

        private void OnMouseOver()
        {
            button.Select();
            if (GameItemInInventory != null) GameItemInInventory.Highlight();
        }

        private void Update()
        {
            if (GameItemInInventory == null || GameItemInInventory.ItemType == null || GameItemInInventory.Quantity <= 0)
            {
                quantityText.transform.parent.gameObject.SetActive(false);
                itemThumbnailImage.enabled = false;
            }
            else
            {
                itemThumbnailImage.enabled = true;
                quantityText.text = GameItemInInventory.Quantity.ToString();
                itemThumbnailImage.sprite = GameItemInInventory.ItemType.ItemSprite;
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseOver();
        }
    }
}