using System;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.DialogueSystem
{
    public class DialogueInteractable : InteractableObject
    {
        [InfoBox("Dialogue to trigger")]
        public string speakerName = "???";
        [TextArea(2, 5), HideIf("UseDialoguePreset")]
        public string[] dialogue;
        [HideIf("UseDialoguePreset")] public bool skippable = true;
        
        [InlineEditor(InlineEditorModes.FullEditor)]
        public DialoguePreset dialoguePreset;
        
        public bool UseDialoguePreset => dialoguePreset != null;

        private Action _unsubscribe;

        public IEnumerable<DialogueItem> GetDialogueItems()
        {
            if (UseDialoguePreset)
            {
                var playableDialogues = dialoguePreset.GetPlayableDialogues();
                
                foreach (var playableDialogue in playableDialogues)
                {
                    var allLines = playableDialogue.GetDialogueLines(speakerName).Select(
                        dialogueLine => new DialogueItem(speakerName, dialogueLine,
                            playableDialogue.PlayConditions.Skippable, gameObject)
                    ).ToArray();

                    if (allLines.Length == 0)
                    {
                        Debug.LogWarning($"No dialogue items for {speakerName}:{playableDialogue.DialogueId.Value}. YOU SHOULD PROBABLY WRITE SOME DIALOGUE.");
                        continue;
                    }
                    
                    var lastDialogueLine = allLines[^1];
                    
                    lastDialogueLine.EmitNpcEvents.Add(playableDialogue.DialogueId);
                    lastDialogueLine.onPlayed.AddListener(playableDialogue.TriggerQuestEvent);

                    foreach (var dialogueItem in allLines)
                    {
                        yield return dialogueItem;
                    }
                }
            }
            else
            {
                foreach (var line in dialogue)
                {
                    yield return new DialogueItem(speakerName, line, skippable, gameObject);
                }
            }
        }
        
        public enum BehaviorEvents
        {
            DialogueStarted,
            DialogueEnded
        }

        private void OnValidate()
        {
            itemName = speakerName;
        }

        private void Start()
        {
            var unsub1 = SceneEventBus.Subscribe<DialogueEvent>(OnDialogueStarted);
            var unsub2 = SceneEventBus.Subscribe<EndDialogueEvent>(OnDialogueEnded);
            
            _unsubscribe = () =>
            {
                unsub1();
                unsub2();
            };
        }

        private void OnDialogueStarted(DialogueEvent obj)
        {
            enabled = false;
        }

        private void OnDialogueEnded(EndDialogueEvent obj)
        {
            enabled = true;
            
            var behaviorTree = GetComponent<Behavior>();
            if (behaviorTree != null)
            {
                behaviorTree.SendEvent(BehaviorEvents.DialogueEnded.ToString());
            }
        }

        protected override void OnInteract(Interactor interactor, dynamic input)
        {
            if (enabled)
            {
                var dialogueItems = GetDialogueItems();
                if (!dialogueItems.Any())
                {
                    Debug.LogWarning($"No dialogue items for {speakerName}.");
                    return;
                }
                
                SceneEventBus.Emit(new DialogueEvent(GetDialogueItems()));
                
                var behaviorTree = GetComponent<Behavior>();
                if (behaviorTree != null)
                {
                    behaviorTree.SendEvent(BehaviorEvents.DialogueStarted.ToString());
                }
            }
        }

        private void OnDestroy()
        {
            _unsubscribe?.Invoke();
        }
    }
}