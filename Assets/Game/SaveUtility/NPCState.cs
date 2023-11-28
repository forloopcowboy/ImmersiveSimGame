using System;

namespace Game.SaveUtility
{
    [Serializable]
    public class NPCState
    {
        public string Identifier;
        public float Health;
        public bool IsHostile;
        public string[] Events;
    }
}