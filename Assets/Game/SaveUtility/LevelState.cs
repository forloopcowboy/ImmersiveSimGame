using System;
using System.Collections.Generic;

namespace Game.SaveUtility
{
    [Serializable]
    public class LevelState
    {
        public int LevelNumber;
        public List<string> Events;
        public List<NPCState> NPCStates;

        public LevelState(int levelNumber, List<string> events, List<NPCState> npcStates)
        {
            LevelNumber = levelNumber;
            Events = events;
            NPCStates = npcStates;
        }
    }
}