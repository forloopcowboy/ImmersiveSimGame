using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
using Game.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.EquipmentSystem
{
   
    public class InventoryContentUIController : MonoBehaviour
    {
        public GameItemInventory Inventory;
        public GameItemThumbnailUIController thumbnailPrefab;
        public List<GameItemThumbnailUIController> thumbnails;
        public Transform root;
        
        public bool IsOpen => root.gameObject.activeSelf;
        
        private void Start()
        {
            if (Inventory == null)
            {
                Debug.LogError("Inventory is null. Initialize it in the inspector or assign it in code.");
            }
            
            if (thumbnailPrefab == null)
            {
                Debug.LogError("thumbnailPrefab is null. Initialize it in the inspector or assign it in code.");
            }
            
            if (root == null)
            {
                Debug.LogError("root is null. Initialize it in the inspector or assign it in code.");
            }

            thumbnails = new List<GameItemThumbnailUIController>();
        }

        public bool ToggleInventory(bool? valueToSet = null)
        {
            bool value = valueToSet.HasValue ? valueToSet.Value : !root.gameObject.activeSelf;
            root.gameObject.SetActive(value);
            
            HandleEquipItemLogic(value);
            HandleCursorLogic(value);
            
            // If no item is highlighted, highlight the first one
            if (value && Inventory.HighlightedItem == null)
            {
                var firstitem = Inventory.ItemsInInventory.FirstOrDefault();
                if (firstitem != null)
                    Inventory.HighlightedItemId = firstitem.Item.Identifier;
            }

            return value;
        }

        private static void HandleCursorLogic(bool value)
        {
            // Lock cursor if opening inventory, unlock if closing
            if (value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private static void HandleEquipItemLogic(bool value)
        {
            // If hiding inventory, stop equipping item
            if (!value)
            {
                SceneEventBus.Emit(new CancelEquipItemEvent());
            }
        }

        private void Update()
        {
            if (Inventory == null)
            {
                throw new System.NullReferenceException("Inventory is null. Initialize it in the inspector or assign it in code.");
            }
            
            if (thumbnailPrefab == null)
            {
                throw new System.NullReferenceException("thumbnailPrefab is null. Initialize it in the inspector or assign it in code.");
            }

            if (Input.GetKeyDown(KeyCode.Escape) && IsOpen)
            {
                ToggleInventory();
            }

            if (IsOpen && Inventory.ItemsInInventory.Count != thumbnails.Count)
            {
                foreach (var thumbnail in thumbnails)
                {
                    Destroy(thumbnail.gameObject);
                }
                
                thumbnails.Clear();
                
                foreach (var item in Inventory.ItemsInInventory)
                {
                    var thumbnail = Instantiate(thumbnailPrefab, root);
                    thumbnail.GameItemInInventory = item;
                    if (thumbnail.GameItemInInventory.IsHighlighted)
                    {
                        thumbnail.button.Select();
                    }
                    
                    thumbnails.Add(thumbnail);
                }
            }
        }

        public bool TryGetSelectedItem(out GameItemInInventory item)
        {
            item = null;
            foreach (var thumbnail in thumbnails)
            {
                if (thumbnail.button.interactable && thumbnail.button.enabled && thumbnail.button.gameObject.activeSelf)
                {
                    item = thumbnail.GameItemInInventory;
                    return true;
                }
            }

            return false;
        }
    }
}