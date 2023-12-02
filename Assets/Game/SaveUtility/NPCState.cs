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
        public SerializedEvent[] Events = Array.Empty<SerializedEvent>();

        public bool HasEventOccurred(Predicate<SerializedEvent> predicate)
        {
            return Array.Exists(Events, predicate);
        }

        /// <summary>
        /// Records event to NPC state.
        /// </summary>
        /// <returns>True if event already occurred.</returns>
        public bool RecordEvent(SerializedEvent serializedEvent)
        {
            var alreadyHappened = HasEventOccurred(serializedEvent.Equals);

            Array.Resize(ref Events, Events.Length + 1);
            Events[Events.Length - 1] = serializedEvent;

            var details = alreadyHappened ? "It already happened" : "";
            Debug.Log($"Event {serializedEvent.EventType} recorded to NPC {NPCName}. {details}");
            
            return alreadyHappened;
        }
    }
}