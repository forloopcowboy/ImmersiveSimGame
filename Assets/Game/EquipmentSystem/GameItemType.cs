using System;
using System.IO;
using Game.Src.IconGeneration;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    [CreateAssetMenu(fileName = "GameItemType", menuName = "GameItem/New GameItemType", order = 0)]
    public class GameItemType : SerializedScriptableObject
    {
        public Sprite ItemSprite;
        public string ItemName;
        public float ItemValue;
        
        [Required("IconPrefab is required to generate IconSprite. If none is provided, ItemSprite will be used. This object should represent the item in the world, but not necessarily have all functions of the object.", InfoMessageType.Warning)] public GameObject IconPrefab;
        
        internal bool IconSpriteGenerated = false;

        private void OnDisable()
        {
            IconSpriteGenerated = false;
        }
    }
}