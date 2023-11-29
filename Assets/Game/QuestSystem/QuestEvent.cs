using System;

namespace Game.QuestSystem
{
    [Serializable]
    public struct QuestEvent
    {
        public static string EVENT_TYPE = "QuestEvent";
        
        public QuestId questId;
        public QuestEventId questEventId;
        public string eventName;
        public string eventDescription;
        
        // override equals operator to compare event name and quest ID
        public static bool operator ==(QuestEvent a, QuestEvent b)
        {
            return a.eventName == b.eventName && a.questId == b.questId && a.questEventId.Value == b.questEventId.Value;
        }

        public static bool operator !=(QuestEvent a, QuestEvent b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QuestEvent) obj);
        }

        public bool Equals(QuestEvent other)
        {
            return Equals(questId, other.questId) && Equals(questEventId, other.questEventId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(questId, questEventId);
        }
    }
}