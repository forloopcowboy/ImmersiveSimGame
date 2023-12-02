using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.QuestSystem
{
    [Serializable]
    public class QuestCondition
    {
        [TabGroup("Quest Condition", order: 10)]
        public List<QuestId> requiredQuestsCompleted = new ();
         
        [TabGroup("Quest Condition", order: 10)]
        public List<QuestId> requiredQuestsActive = new ();
         
        [TabGroup("Quest Condition", order: 10)]
        public List<QuestId> requiredQuestsNotCompleted = new ();
         
        [TabGroup("Quest Event Condition", order: 10)]
        public List<QuestEventId> requiredQuestEventsCompleted = new ();
         
        [TabGroup("Quest Event Condition", order: 10)]
        public List<QuestEventId> requiredQuestEventsNotCompleted = new ();
         
        
        public bool CanTrigger
        {
            get
            {
                foreach (var questId in requiredQuestsCompleted)
                {
                    if (!QuestManager.Singleton.IsQuestCompleted(questId))
                    {
                        Debug.Log($"Quest {questId} not completed. Quest condition not met.");
                        return false;
                    }
                }
                foreach (var questId in requiredQuestsActive)
                {
                    if (!QuestManager.Singleton.IsQuestActive(questId))
                    {
                        Debug.Log($"Quest {questId} not active. Quest condition not met.");
                        return false;
                    }
                }
                foreach (var questId in requiredQuestsNotCompleted)
                {
                    if (QuestManager.Singleton.IsQuestCompleted(questId))
                    {
                        Debug.Log($"Quest {questId} completed. Quest condition not met.");
                        return false;
                    }
                }
                foreach (var questEventId in requiredQuestEventsCompleted)
                {
                    if (!QuestManager.Singleton.HasQuestEvent(questEventId))
                    {
                        Debug.Log($"Quest event {questEventId} not completed. Quest condition not met.");
                        return false;
                    }
                }
                foreach (var questEventId in requiredQuestEventsNotCompleted)
                {
                    if (QuestManager.Singleton.HasQuestEvent(questEventId))
                    {
                        Debug.Log($"Quest event {questEventId} completed. Quest condition not met.");
                        return false;
                    }
                }

                Debug.Log("Quest condition met.");
                return true;
            }
        }
    }
}