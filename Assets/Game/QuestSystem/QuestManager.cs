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
        public List<Quest> Quests = new ();
        public List<Quest> ActiveQuests = new ();
        
        private Action _unsubscribe;
        
        protected override void Awake()
        {
            DontDestroyOnLoad = true;
            base.Awake();
        }

        private void Start()
        {
            _unsubscribe = SceneEventBus.Subscribe<QuestEvent>(HandleQuestEvent);
            
            LoadState();
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
            SyncQuestState();

            GlobalGameState.Singleton.SaveState();
        }

        private void SyncQuestState()
        {
            var savedQuestEvents = LoadSavedQuestEvents();
            Debug.Log("Syncing quest state... Current quest events: " + savedQuestEvents.Count);

            // add all active quest events
            foreach (var quest in ActiveQuests)
            {
                savedQuestEvents.AddRange(quest.questEvents.Select(evt => new SerializedQuestEvent()
                {
                    questId = evt.QuestId,
                    questEventId = evt.QuestEventId,
                    eventName = evt.EventName,
                    eventDescription = evt.EventDescription
                }));
            }

            // remove all old quest events and replace them with the complete set
            GlobalGameState.State.QuestEvents.RemoveAll(
                serializedEvent =>
                    serializedEvent.EventType == QuestEvent.EVENT_TYPE
            );


            GlobalGameState.State.QuestEvents.AddRange(savedQuestEvents.Select(questEvent => new SerializedEvent(
                QuestEvent.EVENT_TYPE,
                JsonUtility.ToJson(questEvent))
            ));

            Debug.Log("Serialized JSON.");
            Debug.Log("Synced quest state... New quest events: " + savedQuestEvents.Count);
        }

        [Button]
        public void LoadState()
        {
            ActiveQuests = new();
            var savedQuestEvents = LoadSavedQuestEvents();

            foreach (var questEvent in savedQuestEvents)
            {
                ProcessQuestEvent(questEvent, false);
            }
        }

        private void HandleQuestEvent(QuestEvent questEvent)
        {
            ProcessQuestEvent(questEvent, true);
            
            // slight delay on saving after quest event to ensure that all quest events are processed
            StartCoroutine(CoroutineHelpers.DelayedAction(1f, SaveState));
        }

        /// <summary>
        /// Given a quest event, either appends quest with event ID to ActiveQuests or adds event to existing quest.
        /// </summary>
        private void ProcessQuestEvent(IQuestEvent questEvent, bool notify)
        {
            Quest quest = Quests.Find(quest => quest.questId == questEvent.QuestId);
            
            if (!ActiveQuests.Exists(q => q.questId == questEvent.QuestId))
            {
                ActiveQuests.Add(quest);
            }
            quest.questEvents.Add(questEvent);
            
            if (!notify) return;
            
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
                    SceneEventBus.Emit(new NotificationEvent($"[Q] Goal updated: {questEvent.EventName}"));
                }));
            }
        }

        private HashSet<SerializedQuestEvent> LoadSavedQuestEvents()
        {
            var serializedSavedEvents = GlobalGameState.State.QuestEvents;
            var savedQuestEvents = new HashSet<SerializedQuestEvent>(
                serializedSavedEvents.Where(
                    serializedEvent => 
                        serializedEvent.EventType == QuestEvent.EVENT_TYPE
                ).Select(
                    json => 
                        JsonUtility.FromJson<SerializedQuestEvent>(json.EventData)
                )
            );

            return savedQuestEvents;
        }

        public bool IsQuestCompleted(QuestId questId)
        {
            return ActiveQuests.Exists(quest => quest.questId == questId && quest.IsCompleted);
        }


        public bool HasQuestEvent(QuestEventId questEventId, Predicate<IQuestEvent> predicate = null)
        {
            return ActiveQuests.Exists(quest => quest.questEvents.Exists(questEvent => questEvent.QuestEventId == questEventId && (predicate == null || predicate(questEvent))));
        }

        public bool IsQuestActive(QuestId questId)
        {
            return ActiveQuests.Exists(quest => quest.questId == questId);
        }
    }
}