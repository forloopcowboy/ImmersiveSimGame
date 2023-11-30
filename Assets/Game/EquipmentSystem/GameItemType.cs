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
        [SerializeField]
        private string _identifier = "None";
        public string ItemName;
        public Sprite ItemSprite;
        public float ItemValue;
        
        [Required("IconPrefab is required to generate IconSprite. If none is provided, ItemSprite will be used. This object should represent the item in the world, but not necessarily have all functions of the object.", InfoMessageType.Warning)] public GameObject IconPrefab;
        
        internal bool IconSpriteGenerated = false;

        public void GenerateIconSprite()
        {
            var itemType = this;
            
            if (itemType != null && itemType.IconPrefab != null && !itemType.IconSpriteGenerated) IconPrinter.Singleton.GetIcon(itemType.IconPrefab, icon =>
            {
                if (!itemType.IconSpriteGenerated)
                    itemType.ItemSprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
                itemType.IconSpriteGenerated = true;
                Debug.Log("Created item sprite.");
            });
        }
        
        private void OnDisable()
        {
            IconSpriteGenerated = false;
        }

        [Button]
        public void GenerateIdentifier()
        {
            _identifier = Guid.NewGuid().ToString();
        }
        

        public string Identifier => _identifier;
    }
}