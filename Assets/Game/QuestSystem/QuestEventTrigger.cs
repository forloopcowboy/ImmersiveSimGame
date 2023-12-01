using System;
using System.Collections.Generic;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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

        [TabGroup("TriggerCondition", order: 10)] public List<QuestId> requiredQuestsCompleted;
        [TabGroup("TriggerCondition", order: 10)] public List<QuestId> requiredQuestsActive;
        [TabGroup("TriggerCondition", order: 10)] public List<QuestId> requiredQuestsNotCompleted;
        [FormerlySerializedAs("requiredQuestEvents")] [TabGroup("TriggerCondition", order: 10)] public List<QuestEventId> requiredQuestEventsCompleted;
        [TabGroup("TriggerCondition", order: 10)] public List<QuestEventId> requiredQuestEventsNotCompleted;

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
                foreach (var questId in requiredQuestsActive)
                {
                    if (!QuestManager.Singleton.IsQuestActive(questId))
                    {
                        Debug.Log($"Quest {questId} not active. Cannot trigger event {questEvent.eventName}.");
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
                foreach (var questEventId in requiredQuestEventsCompleted)
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