using System;
using Game.SaveUtility;
using Game.Src.IconGeneration;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    [CreateAssetMenu(fileName = "GameItemType", menuName = "GameItem/New GameItemType", order = 0)]
    public class GameItemType : SerializedScriptableObject, IHasIdentifier
    {
        [SerializeField, TabGroup("Info"), ReadOnly]
        private string _identifier = "None";
        [TabGroup("Info")]
        public string ItemName;
        [TabGroup("Thumbnail")]
        public Sprite ItemSprite;
        [TabGroup("Info")]
        public float ItemValue;
        
        [TabGroup("Thumbnail"), Required("IconPrefab is required to generate IconSprite. If none is provided, ItemSprite will be used. This object should represent the item in the world, but not necessarily have all functions of the object.", InfoMessageType.Warning)] 
        public GameObject IconPrefab;
        [TabGroup("Thumbnail"), SerializeField, ReadOnly]
        internal bool IconSpriteGenerated;
        
        [Button]
        public void InitializeItem()
        {
            GenerateIdentifier();
            GenerateIconSprite();
        }

        [Button(SdfIconType.Bricks), TabGroup("Info")]
        private void GenerateIdentifier()
        {
            _identifier = $"{ItemName.Replace(" ", "-")}-{Guid.NewGuid().ToString().Split("-")[0]}";
        }
        
        public void GenerateIconSprite()
        {
            var itemType = this;

            if (itemType.IconPrefab != null && !itemType.IconSpriteGenerated) ForceGenerateIconSprite();
            else Debug.Log("IconPrefab is null or IconSprite has already been generated.");
        }

        [Button(SdfIconType.Image), TabGroup("Thumbnail")]
        public void ForceGenerateIconSprite()
        {
            var itemType = this;
           
            IconPrinter.Singleton.GetIcon(itemType.IconPrefab, icon =>
            {
                var itemIdOrName = _identifier == "None" ? ItemName : _identifier;

                if (_identifier == "None" || String.IsNullOrEmpty(_identifier)) 
                    Debug.LogError($"Identifier is not set for item {itemIdOrName}. Asset will not be created. Please set the identifier to avoid recreating sprites every time the game is loaded.");
                else
                {
#if UNITY_EDITOR
                    if (ItemSprite != null && UnityEditor.AssetDatabase.Contains(ItemSprite))
                    {
                        Debug.Log($"GameItem icon for {itemIdOrName} already exists. Overwriting...");
                    }

                    var path = $"Assets/Game/GameItems/Icons/{itemIdOrName}.png";
                    
                    // Create sprite
                    Sprite sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
                    
                    ItemSprite = sprite.SaveSpriteToEditorPath(path);
                    
                    itemType.IconSpriteGenerated = true;
                    UnityEditor.Selection.activeObject = ItemSprite;
                    
                    // Save game item type asset
                    UnityEditor.EditorUtility.SetDirty(itemType);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    
                    Debug.Log($"Created sprite for item {itemIdOrName}.");
#endif
                }
            });
        }
        
        public string Identifier => _identifier;
    }
}