using System;
using UnityEngine;

namespace Game.SaveUtility
{
    [Serializable]
    public class NPCState
    {
        public string NPCName;
        public string Identifier;
        public float Health;
        public Vector3 Position;
        public Vector3 Rotation;
        public bool IsHostile;
        public string[] Events;
    }
}