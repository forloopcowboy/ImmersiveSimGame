using System;

namespace Game.SaveUtility
{
    [Serializable]
    public class GameItemState
    {
        public string ItemId;
        public bool IsPickedUp;

        public GameItemState(string itemId, bool isPickedUp)
        {
            ItemId = itemId;
            IsPickedUp = isPickedUp;
            
        }
    }
}