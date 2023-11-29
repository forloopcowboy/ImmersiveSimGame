using System;
using Game.SaveUtility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.EquipmentSystem
{
    [CreateAssetMenu(fileName = "GameItemType", menuName = "GameItem/New GameItemType", order = 0)]
    public class GameItemType : SerializedScriptableObject, IHasIdentifier
    {
        [ReadOnly, SerializeField]
        private string _identifier;
        public Sprite ItemSprite;
        public string ItemName;
        public float ItemValue;
        
        [Required("IconPrefab is required to generate IconSprite. If none is provided, ItemSprite will be used. This object should represent the item in the world, but not necessarily have all functions of the object.", InfoMessageType.Warning)] public GameObject IconPrefab;
        
        internal bool IconSpriteGenerated = false;

        private void OnValidate()
        {
            if (String.IsNullOrEmpty(_identifier))
            {
                _identifier = Guid.NewGuid().ToString();
            }
        }

        private void OnDisable()
        {
            IconSpriteGenerated = false;
        }


        public string Identifier => _identifier;
    }
}