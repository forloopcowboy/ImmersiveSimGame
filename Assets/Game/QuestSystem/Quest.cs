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
        public List<IQuestEvent> questEvents = new ();
        [Tooltip("Events that need to happen in order to complete this quest.")]
        public List<QuestEventId> requiredToComplete = new ();

        public bool IsCompleted => requiredToComplete.TrueForAll(e => questEvents.Exists(qe => qe.QuestId == questId && qe.QuestEventId == e));
    }
}