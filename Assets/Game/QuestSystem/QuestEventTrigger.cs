using System;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;

namespace Game.QuestSystem
{
    /// <summary>
    /// Use this to trigger QuestEvents.
    /// </summary>
    [Serializable]
    public class QuestEventTrigger
    {
        public bool isEnabled;
        
        [Tooltip("The event to trigger."), TabGroup("Quest")]
        public QuestEvent questEvent;

        [TabGroup("Quest")] public List<QuestId> requiredQuestsCompleted;
        [TabGroup("Quest")] public List<QuestId> requiredQuestsNotCompleted;
        [TabGroup("Quest")] public List<QuestEventId> requiredQuestEvents;
        [TabGroup("Quest")] public List<QuestEventId> requiredQuestEventsNotCompleted;
        
        public bool CanTrigger
        {
            get
            {
                foreach (var questId in requiredQuestsCompleted)
                {
                    if (!QuestManager.Singleton.IsQuestCompleted(questId))
                    {
                        return false;
                    }
                }
                foreach (var questId in requiredQuestsNotCompleted)
                {
                    if (QuestManager.Singleton.IsQuestCompleted(questId))
                    {
                        return false;
                    }
                }
                foreach (var questEventId in requiredQuestEvents)
                {
                    if (!QuestManager.Singleton.HasQuestEvent(questEventId))
                    {
                        return false;
                    }
                }
                foreach (var questEventId in requiredQuestEventsNotCompleted)
                {
                    if (QuestManager.Singleton.HasQuestEvent(questEventId))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        [Button, InfoBox("Triggers event if meets criteria above."), TabGroup("Quest")]
        public void Trigger()
        {
            if (isEnabled && CanTrigger) SceneEventBus.Emit(questEvent);
        }
    }
}