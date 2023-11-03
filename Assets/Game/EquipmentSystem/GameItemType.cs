using UnityEngine;

namespace Game.EquipmentSystem
{
    [CreateAssetMenu(fileName = "GameItemType", menuName = "GameItem/New GameItem Type", order = 0)]
    public class GameItemType : ScriptableObject
    {
        public Sprite ItemSprite;
        public string ItemName;
        public float ItemValue;
    }
}