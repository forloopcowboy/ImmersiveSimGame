using System;
using Game.Utils;

namespace Game.SaveUtility
{
    [Serializable]
    public class SerializedEvent
    {
        public string EventType;
        public string EventData;
        
        public SerializedEvent(string eventType, string eventData)
        {
            EventType = eventType;
            EventData = eventData;
        }
        
        public bool Equals(SerializedEvent other)
        {
            return EventType == other.EventType && EventData == other.EventData;
        }
    }
}