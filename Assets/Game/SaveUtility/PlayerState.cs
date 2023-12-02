using System;
using System.Collections.Generic;

namespace Game.SaveUtility
{
    [Serializable]
    public class PlayerState
    {
        public float Health;
        public List<SerializedEvent> Events = new ();
        public SerializedItemData[] Inventory;
        public int HeldItemIndex;
        public int[] EquippedItems;
        public bool IsInitialized;
    }
    
    [Serializable]
    public class SerializedItemData
    {
        public string Identifier;
        public int Quantity;

        public SerializedItemData(string identifier, int quantity)
        {
            Identifier = identifier;
            Quantity = quantity;
        }
    }
}