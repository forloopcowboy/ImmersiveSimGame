using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SaveUtility
{
    [Serializable]
    public class LevelState
    {
        public int LevelNumber;
        public List<SerializedEvent> Events;
        public List<NPCState> NPCStates;
        public List<GameItemState> GameItemStates;

        public bool PlayerLocationInitialized;
        public Vector3 PlayerPosition;
        public Vector3 PlayerRotation;
        
        public bool IsItemPickedUp(string itemId)
        {
            var item = GameItemStates.Find(item => item.ItemId == itemId);
            if (item != null)
            {
                return item.IsPickedUp;
            }
            
            GameItemStates.Add(new GameItemState(itemId, false));
            return false;
        }
        
        public void SetItemPickedUp(string itemId)
        {
            var item = GameItemStates.Find(item => item.ItemId == itemId);
            if (item == null)
            {
                GameItemStates.Add(item = new GameItemState(itemId, true));
            }

            item.IsPickedUp = true;
        }

        public LevelState(int levelNumber, List<SerializedEvent> events, List<NPCState> npcStates, List<GameItemState> itemStates)
        {
            LevelNumber = levelNumber;
            Events = events;
            NPCStates = npcStates;
            GameItemStates = itemStates;
        }
    }
}