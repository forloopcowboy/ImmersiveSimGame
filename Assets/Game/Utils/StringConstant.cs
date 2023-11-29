using UnityEngine;

namespace Game.Utils
{
    [CreateAssetMenu(fileName = "Constants", menuName = "String Constant", order = 0)]
    public class StringConstant : ScriptableObject
    {
        public string Value = "default";
        
        public static implicit operator string(StringConstant constant) => constant != null ? constant.Value : null;
    }
}