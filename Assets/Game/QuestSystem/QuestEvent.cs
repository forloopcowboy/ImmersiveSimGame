using System;

namespace Game.QuestSystem
{
    [Serializable]
    public struct QuestEvent : IQuestEvent
    {
        public static string EVENT_TYPE = "QuestEvent";
        
        public QuestId questId;
        public QuestEventId questEventId;
        public string eventName;
        public string eventDescription;

        public SerializedQuestEvent ToSerialized()
        {
            return new SerializedQuestEvent
            {
                questId = questId.Value,
                questEventId = questEventId.Value,
                eventName = eventName,
                eventDescription = eventDescription
            };
        }

        public string QuestId => questId; 
        public string QuestEventId => questEventId; 
        public string EventName => eventName; 
        public string EventDescription => eventDescription; 
    }
    
    [Serializable]
    public struct SerializedQuestEvent : IQuestEvent
    {
        public string questId;
        public string questEventId;
        public string eventName;
        public string eventDescription;
        
        public string QuestId => questId;
        public string QuestEventId => questEventId;
        public string EventName => eventName;
        public string EventDescription => eventDescription;
    }
    
    public interface IQuestEvent
    {
        public string QuestId { get; }
        public string QuestEventId { get; }
        public string EventName { get; }
        public string EventDescription { get; }
    }
}