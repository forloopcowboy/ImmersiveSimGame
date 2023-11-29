using System;
using System.Collections;
using System.Collections.Generic;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.EquipmentSystem
{
    public enum InventoryIndexingDirection
    {
        UP,
        DOWN,
        NONE
    }
    
    public class InventoryContentUIController : MonoBehaviour
    {
        public GameItemInventory Inventory;
        public GameItemThumbnailUIController thumbnailPrefab;
        public List<GameItemThumbnailUIController> thumbnails;
        public Transform root;
        
        public bool IsOpen => root.gameObject.activeSelf;

        private InventoryIndexingDirection _requestedDirection;
        
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

            StartCoroutine(HandleScrollingMenuDirectionCoroutine());
            
            thumbnails = new List<GameItemThumbnailUIController>();
        }

        public bool ToggleInventory()
        {
            bool value = !root.gameObject.activeSelf;
            root.gameObject.SetActive(value);
            return value;
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

            if (IsOpen)
            {
                var direction = InventoryIndexingDirection.NONE;
                var value = Input.GetAxis("Mouse ScrollWheel");
                if (value > 0)
                {
                    direction = InventoryIndexingDirection.UP;
                }
                else if (value < 0)
                {
                    direction = InventoryIndexingDirection.DOWN;
                }
                
                _requestedDirection = direction;
            }
            else _requestedDirection = InventoryIndexingDirection.NONE;

            if (Inventory.ItemsInInventory.Count != thumbnails.Count)
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
                    thumbnails.Add(thumbnail);
                }
            }
        }

        public IEnumerator HandleScrollingMenuDirectionCoroutine()
        {
            var interval = new WaitForSeconds(.01f);

            while (true)
            {
                if (IsOpen)
                {
                    var nextIndex = -1;
                    if (_requestedDirection != InventoryIndexingDirection.NONE)
                    {
                        // get current index
                        var currentIndex = -1;
                        for (var i = 0; i < thumbnails.Count; i++)
                        {
                            var thumbnail = thumbnails[i];
                            if (thumbnail.button.gameObject == EventSystem.current.currentSelectedGameObject)
                            {
                                currentIndex = i;
                                break;
                            }
                        }

                        // get next index
                        switch (_requestedDirection)
                        {
                            case InventoryIndexingDirection.DOWN:
                                nextIndex = Mathf.Min(currentIndex + 1, thumbnails.Count - 1);
                                break;
                            case InventoryIndexingDirection.UP:
                                nextIndex = Mathf.Max(currentIndex - 1, 0);
                                break;
                            case InventoryIndexingDirection.NONE:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    Debug.Log($"NextIndex: {nextIndex}, Requested Direction: {_requestedDirection}");

                    // set button active
                    if (nextIndex != -1) for (var i = 0; i < thumbnails.Count; i++)
                    {
                        var thumbnail = thumbnails[i];
                        if (thumbnail.GameItemInInventory?.Quantity <= 0)
                        {
                            nextIndex++;
                        }
                        
                        if (i == nextIndex && thumbnail != null && thumbnail.GameItemInInventory?.Item != null)
                        {
                            Debug.Log($"Item to select: {thumbnail.GameItemInInventory.Item.name}");
                            thumbnail.button.Select();
                            SceneEventBus.Emit(new NotificationEvent(thumbnail.GameItemInInventory.Item.name));
                        }
                        else
                        {
                            thumbnail.button.OnDeselect(null);
                        }
                    }
                }
                else yield return null;

                yield return interval;
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