using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;

namespace Game.QuestSystem
{
    [Serializable]
    public class Quest
    {
        public QuestId questId;
        public string questName;
        public string questDescription;
        [Tooltip("Events that have happened in this quest.")]
        public List<QuestEvent> questEvents;
        [Tooltip("Events that need to happen in order to complete this quest.")]
        public List<QuestEventId> requiredToComplete;

        public bool IsCompleted => requiredToComplete.TrueForAll(e => questEvents.Exists(qe => qe.questId == questId && qe.questEventId == e));
    }
}