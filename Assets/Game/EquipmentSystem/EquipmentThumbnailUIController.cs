using System;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.EquipmentSystem
{
    public class EquipmentThumbnailUIController : MonoBehaviour
    {
        [Required]
        public GameItemInventory Inventory;
        public LayoutGroup root;
        public GameItemThumbnailUIController[] EquippedUiItems;
        
        private bool isListeningForNumberInput = false;
        private GameItemInInventory itemToEquip = null;
        private Action unsubscribe;

        private void OnValidate()
        {
            EquippedUiItems = root.GetComponentsInChildren<GameItemThumbnailUIController>();
            if (EquippedUiItems == null)
            {
                Debug.LogWarning("EquippedUiItems is null. Initialize it in the inspector or assign it in code.");
            }
            if (EquippedUiItems?.Length != 10)
            {
                Debug.LogWarning("EquippedUiItems length is not 10. Make sure there are 10 children of the root.");
            }

        }

        public void OnEnable()
        {
            if (root == null)
            {
                throw new NullReferenceException("LayoutGroup is null. Initialize it in the inspector or assign it in code.");
            }

            EquippedUiItems = root.GetComponentsInChildren<GameItemThumbnailUIController>();
            if (EquippedUiItems == null)
            {
                throw new NullReferenceException("EquippedUiItems is null. Initialize it in the inspector or assign it in code.");
            }
            if (EquippedUiItems.Length != 10)
            {
                throw new Exception("EquippedUiItems length is not 10. Make sure there are 10 children of the root.");
            }
            
            unsubscribe = SceneEventBus.Subscribe<TryEquipItemEvent>(OnTryEquipItem);
        }

        private void OnDisable()
        {
            unsubscribe?.Invoke();
        }

        private void OnTryEquipItem(TryEquipItemEvent obj)
        {
            isListeningForNumberInput = true;
            itemToEquip = obj.Item;
        }

        public void Update()
        {
            var equippedItems = Inventory.EquippedItems;

            HandleEquipItemInput();
            
            for (var i = 0; i < equippedItems.Count; i++)
            {
                var item = equippedItems[i];
                var uiItem = EquippedUiItems[i];
                
                // Disable button functionality to control highlight image to show active equipped item.
                uiItem.button.enabled = false;
                uiItem.button.targetGraphic.enabled = false;

                if (isListeningForNumberInput)
                {
                    uiItem.equipIndicatorText.enabled = true;
                    uiItem.equipIndicatorText.text = (i + 1 == 10 ? 0 : i + 1).ToString();
                }
                else
                {
                    uiItem.equipIndicatorText.enabled = false;
                }

                if (item == null)
                {
                    uiItem.GameItemInInventory = null;
                }
                else if (item.Item is EquipableItemType equipableItem)
                {
                    uiItem.GameItemInInventory = item;
                }
            }
        }

        private void HandleEquipItemInput()
        {
            if (isListeningForNumberInput)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Inventory.EquipItem(itemToEquip, 0);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Inventory.EquipItem(itemToEquip, 1);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Inventory.EquipItem(itemToEquip, 2);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Inventory.EquipItem(itemToEquip, 3);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    Inventory.EquipItem(itemToEquip, 4);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    Inventory.EquipItem(itemToEquip, 5);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    Inventory.EquipItem(itemToEquip, 6);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    Inventory.EquipItem(itemToEquip, 7);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    Inventory.EquipItem(itemToEquip, 8);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    Inventory.EquipItem(itemToEquip, 9);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
            }
        }
    }
    
    /// <summary>
    /// Emitted when user tries to equip an item.
    /// Begins listening for user to press number button from 1-0.
    /// </summary>
    public class TryEquipItemEvent
    {
        public GameItemInInventory Item;
        
        public TryEquipItemEvent(GameItemInInventory item)
        {
            Item = item;
        }
    }
}