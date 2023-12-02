using System;
using System.Collections.Generic;
using Game.QuestSystem;
using Game.SaveUtility;
using Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.DialogueSystem
{
    [Serializable]
    public class Dialogue
    {
        public StringConstant DialogueId;
        public PlayConditions PlayConditions = new();

        public bool TriggersQuestEvent;
        [ShowIf("TriggersQuestEvent"), InlineEditor(InlineEditorModes.FullEditor)] public QuestEventPreset QuestEventPreset;
        
        [SerializeField, TextArea(1, 5), InfoBox("The dialogue lines to be played when this dialogue is triggered. You may use the following tags to insert data into the dialogue: $speakerName")]
        private string[] _dialogueLines = Array.Empty<string>();
        
        public bool CanPlay => PlayConditions.CanTrigger;
        
        /// <summary>
        /// Returns dialogue lines with data inserted.
        /// </summary>
        public IEnumerable<string> GetDialogueLines(string speakerName)
        {
            foreach (var dialogueLine in _dialogueLines)
            {
                yield return dialogueLine.Replace("$speakerName", speakerName);
            }
        }
        
        public void TriggerQuestEvent()
        {
            if (TriggersQuestEvent)
            {
                if (QuestEventPreset.questEventTrigger.CanTrigger)
                    Debug.Log($"Triggering {QuestEventPreset.questEventTrigger.questEvent.eventName} QuestEvent from dialogue.");
                else Debug.Log($"Cannot trigger {QuestEventPreset.questEventTrigger.questEvent.eventName} QuestEvent from dialogue.");
                
                QuestEventPreset.questEventTrigger.Trigger();
            }
        }
    }

    [Serializable]
    public class PlayConditions
    {
        public bool Skippable = true;
        
        [InfoBox("Defined events to be present for the given NPC to be able to play this dialogue.")]
        public List<NpcDialogueEventCondition> RequiredNpcEvents = new();
        [InfoBox("Defined events to be NOT present for the given NPC to be able to play this dialogue.")]
        public List<NpcDialogueEventCondition> RequiredNpcEventsNotOcurred = new();

        public QuestCondition QuestCondition = new();
        
        public bool CanTrigger => QuestCondition.CanTrigger && NpcEventsConditionMet;
        
        public bool NpcEventsConditionMet
        {
            get
            {
                foreach (var eventCondition in RequiredNpcEvents)
                {
                    if (!GlobalGameState.State.HasNpcEventOccurred(eventCondition.NpcId, eventCondition.EventCondition))
                    {
                        Debug.Log($"Npc event {eventCondition.Event.Value} not occurred. NPC Event condition not met.");
                        return false;
                    }
                }
                foreach (var eventCondition in RequiredNpcEventsNotOcurred)
                {
                    if (GlobalGameState.State.HasNpcEventOccurred(eventCondition.NpcId, eventCondition.EventCondition))
                    {
                        Debug.Log($"Npc event {eventCondition.Event.Value} occurred. NPC Event condition not met.");
                        return false;
                    }
                }

                Debug.Log("NPC Event condition met.");
                return true;
            }
        }
    }

    [Serializable]
    public class NpcDialogueEventCondition
    {
        public string NpcId;
        public StringConstant Event;
        
        public Predicate<SerializedEvent> EventCondition => serializedEvent => { return serializedEvent.EventType == "DialogueEvent" && serializedEvent.EventData == Event.Value; };
        
        [Button]
        public void GetIdFromActor(ISerializedActor actor)
        {
            NpcId = actor.GetIdentifier();
        }
    }
}