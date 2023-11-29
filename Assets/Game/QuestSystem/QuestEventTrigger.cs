using System;
using System.Collections.Generic;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.QuestSystem
{
    /// <summary>
    /// Use this to trigger QuestEvents.
    /// </summary>
    [Serializable]
    public class QuestEventTrigger
    {
        public bool isEnabled;
        
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The event to trigger."), TabGroup("Quest")]
        public QuestEvent questEvent;

        [TabGroup("Quest")] public List<QuestId> requiredQuestsCompleted;
        [TabGroup("Quest")] public List<QuestId> requiredQuestsNotCompleted;
        [TabGroup("Quest")] public List<QuestEventId> requiredQuestEvents;
        [TabGroup("Quest")] public List<QuestEventId> requiredQuestEventsNotCompleted;

        public QuestEventTrigger(bool isEnabled)
        {
            this.isEnabled = isEnabled;
        }

        public bool CanTrigger
        {
            get
            {
                foreach (var questId in requiredQuestsCompleted)
                {
                    if (!QuestManager.Singleton.IsQuestCompleted(questId))
                    {
                        Debug.Log($"Quest {questId} not completed. Cannot trigger event {questEvent.eventName}.");
                        return false;
                    }
                }
                foreach (var questId in requiredQuestsNotCompleted)
                {
                    if (QuestManager.Singleton.IsQuestCompleted(questId))
                    {
                        Debug.Log($"Quest {questId} completed. Cannot trigger event {questEvent.eventName}.");
                        return false;
                    }
                }
                foreach (var questEventId in requiredQuestEvents)
                {
                    if (!QuestManager.Singleton.HasQuestEvent(questEventId))
                    {
                        Debug.Log($"Quest event {questEventId} not completed. Cannot trigger event {questEvent.eventName}.");
                        return false;
                    }
                }
                foreach (var questEventId in requiredQuestEventsNotCompleted)
                {
                    if (QuestManager.Singleton.HasQuestEvent(questEventId))
                    {
                        Debug.Log($"Quest event {questEventId} completed. Cannot trigger event {questEvent.eventName}.");
                        return false;
                    }
                }

                Debug.Log("Can trigger: " + questEvent.eventName);
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