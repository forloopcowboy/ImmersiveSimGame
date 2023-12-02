using System;
using System.Runtime.Serialization;
using UnityEngine;

namespace Game.Utils
{
    [CreateAssetMenu(fileName = "Constants", menuName = "String Constant", order = 0), Serializable]
    public class StringConstant : ScriptableObject, ISerializable
    {
        public string Value = "default";
        
        public static implicit operator string(StringConstant constant) => constant != null ? constant.Value : null;
        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Value", Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}