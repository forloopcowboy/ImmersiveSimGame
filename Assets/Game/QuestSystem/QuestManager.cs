using System;
using System.Collections.Generic;
using System.Linq;
using Game.SaveUtility;
using Game.Src.EventBusModule;
using Game.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game.QuestSystem
{
    public class QuestManager : SingletonMonoBehaviour<QuestManager>
    {
        public List<Quest> Quests;
        public List<Quest> ActiveQuests;
        
        private Action _unsubscribe;
        
        protected override void Awake()
        {
            DontDestroyOnLoad = true;
            base.Awake();
        }

        private void Start()
        {
            _unsubscribe = SceneEventBus.Subscribe<QuestEvent>(HandleQuestEvent);
        }

        private void OnDisable()
        {
            _unsubscribe?.Invoke();
        }

        /// <summary>
        /// Saves quest state to GlobalGameState.
        /// </summary>
        [Button]
        public void SaveState()
        {
            var serializedSavedEvents = GlobalGameState.State.Events;
            var savedQuestEvents = LoadSavedQuestEvents(); 
            
            // add all active quest events
            foreach (var quest in ActiveQuests)
            {
                savedQuestEvents.AddRange(quest.questEvents);
            }
            
            // remove all old quest events and replace them with the complete set
            serializedSavedEvents.RemoveAll(
                serializedEvent => 
                    serializedEvent.EventType == QuestEvent.EVENT_TYPE
                );

            serializedSavedEvents.AddRange(savedQuestEvents.Select(questEvent => new SerializedEvent
            {
                EventType = QuestEvent.EVENT_TYPE,
                EventData = JsonUtility.ToJson(questEvent)
            }));
            
            GlobalGameState.Singleton.SaveState();
        }

        [Button]
        public void LoadState()
        {
            ActiveQuests = new();
            var savedQuestEvents = LoadSavedQuestEvents();

            foreach (var questEvent in savedQuestEvents)
            {
                HandleQuestEvent(questEvent);
            }
        }

        /// <summary>
        /// Given a quest event, either appends quest with event ID to ActiveQuests or adds event to existing quest.
        /// </summary>
        private void HandleQuestEvent(QuestEvent questEvent)
        {
            Quest quest = Quests.First(quest => quest.questId == questEvent.questId);
            
            if (!ActiveQuests.Exists(q => q.questId == questEvent.questId))
            {
                ActiveQuests.Add(quest);
            }
            
            quest.questEvents.Add(questEvent);
            
            if (quest.IsCompleted)
            {
                StartCoroutine(CoroutineHelpers.DelayedAction(1.5f, () =>
                {
                    SceneEventBus.Emit(new NotificationEvent($"[Q] Completed: {quest.questName}"));
                }));
            }
            else
            {
                StartCoroutine(CoroutineHelpers.DelayedAction(1.5f, () =>
                {
                    SceneEventBus.Emit(new NotificationEvent($"[Q] Goal updated: {questEvent.eventName}"));
                }));
                
            }
        }

        private HashSet<QuestEvent> LoadSavedQuestEvents()
        {
            var serializedSavedEvents = GlobalGameState.State.Events;
            var savedQuestEvents = new HashSet<QuestEvent>(
                serializedSavedEvents.Where(
                    serializedEvent => 
                        serializedEvent.EventType == QuestEvent.EVENT_TYPE
                ).Select(
                    json => 
                        JsonUtility.FromJson<QuestEvent>(json.EventData)
                )
            );

            return savedQuestEvents;
        }

        public bool IsQuestCompleted(QuestId questId)
        {
            return ActiveQuests.Exists(quest => quest.questId == questId && quest.IsCompleted);
        }


        public bool HasQuestEvent(QuestEventId questEventId)
        {
            return ActiveQuests.Exists(quest => quest.questEvents.Exists(questEvent => questEvent.questEventId == questEventId));
        }
    }
}