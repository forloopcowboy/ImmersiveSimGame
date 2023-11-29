using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.SaveUtility
{
    [Serializable]
    public class LevelState
    {
        public int LevelNumber;
        public List<string> Events;
        public List<NPCState> NPCStates;

        public bool PlayerLocationInitialized;
        public Vector3 PlayerPosition;
        public Vector3 PlayerRotation;

        public LevelState(int levelNumber, List<string> events, List<NPCState> npcStates)
        {
            LevelNumber = levelNumber;
            Events = events;
            NPCStates = npcStates;
        }
    }
}