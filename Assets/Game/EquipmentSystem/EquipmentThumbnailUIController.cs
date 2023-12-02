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
            
            var unsub1 = SceneEventBus.Subscribe<TryEquipItemEvent>(OnTryEquipItem);
            var unsub2 = SceneEventBus.Subscribe<CancelEquipItemEvent>(OnCancelEquipItem);
            
            AddListenerToEquipItemOnClick();

            unsubscribe = () =>
            {
                unsub1();
                unsub2();
            };
        }

        private void AddListenerToEquipItemOnClick()
        {
            // Subscribe to clicks on equipped item thumbnails to allow equipping via clicks
            for (int i = 0; i < EquippedUiItems.Length; i++)
            {
                var uiIndex = i;
                var uiItem = EquippedUiItems[uiIndex];
                uiItem.button.onClick.AddListener(() =>
                {
                    if (isListeningForNumberInput && itemToEquip != null)
                    {
                        Debug.Log($"Equipping {itemToEquip.Item.ItemName} @ slot {uiIndex + 1} via click.");
                        Inventory.EquipItem(itemToEquip, uiIndex);
                        isListeningForNumberInput = false;
                        itemToEquip = null;
                    }
                });
            }
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
        
        private void OnCancelEquipItem(CancelEquipItemEvent obj)
        {
            isListeningForNumberInput = false;
            itemToEquip = null;
        }


        public void Update()
        {
            var equippedItems = Inventory.EquippedItems;

            HandleEquipItemInput();
            
            for (var i = 0; i < EquippedUiItems.Length; i++)
            {
                var item = i < equippedItems.Length && equippedItems[i] > -1 && equippedItems[i] < Inventory.ItemsInInventory.Count ?
                    Inventory.ItemsInInventory[equippedItems[i]] :
                    null;
                var uiItem = EquippedUiItems[i];
                
                if (uiItem == null)
                    continue;

                if (item == Inventory.ActivelyHeldItem && item != null && item.Quantity > 0)
                {
                    uiItem.button.targetGraphic.enabled = true;
                }

                if (isListeningForNumberInput)
                {
                    uiItem.equipIndicatorText.enabled = true;
                    uiItem.equipIndicatorText.text = (i + 1 == 10 ? 0 : i + 1).ToString();
                    
                    // Enable button functionality to be able to equip item via click.
                    uiItem.button.enabled = true;
                    uiItem.button.targetGraphic.enabled = true;
                    
                }
                else
                {
                    uiItem.equipIndicatorText.enabled = false;
                    
                    // Disable button functionality to control highlight image to show active equipped item.
                    uiItem.button.enabled = false;
                    uiItem.button.targetGraphic.enabled = false;
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
            var i = -1;
                
            if (Input.GetKeyDown(KeyCode.Alpha1))
                i = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                i = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                i = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                i = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                i = 4;
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                i = 5;
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                i = 6;
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                i = 7;
            else if (Input.GetKeyDown(KeyCode.Alpha9))
                i = 8;
            else if (Input.GetKeyDown(KeyCode.Alpha0))
                i = 9;

            if (i > -1) {
                if (isListeningForNumberInput)
                {
                    Inventory.EquipItem(itemToEquip, i);
                    isListeningForNumberInput = false;
                    itemToEquip = null;
                }
                else
                {
                    var itemIndex = Inventory.EquippedItems[i];

                    if (itemIndex > -1 && itemIndex < Inventory.ItemsInInventory.Count)
                        Inventory.HoldItem(Inventory.ItemsInInventory[itemIndex]);
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
    
    public class CancelEquipItemEvent
    {
        
    }
}