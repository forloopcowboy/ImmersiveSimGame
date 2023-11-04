using Game.EquipmentSystem;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.DoorSystem
{
    [CreateAssetMenu(fileName = "KeyItemType", menuName = "GameItem/New Key Type", order = 0)]
    public class KeyItemType : GameItemType
    {
        [Required]
        public StringConstant password;
        
        public static implicit operator string(KeyItemType key) => key.password;
    }
}