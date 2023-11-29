using System;
using Game.Utils;

namespace Game.SaveUtility
{
    [Serializable]
    public class SerializedEvent
    {
        public string EventType;
        public string EventData;
    }
}