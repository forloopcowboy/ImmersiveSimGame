using System;
using System.Linq;
using BehaviorDesigner.Runtime;
using Game.InteractionSystem;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;

namespace Game.DialogueSystem
{
    public class DialogueInteractor : InteractableObject
    {
        [InfoBox("Dialogue to trigger")]
        public string speakerName = "???";
        public string[] dialogue;

        private Action _unsubscribe;
        
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
            
            var behaviorTree = GetComponent<Behavior>();
            if (behaviorTree != null)
            {
                behaviorTree.SendEvent(BehaviorEvents.DialogueStarted.ToString());
            }
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

        public override void Interact(dynamic input)
        {
            if (enabled) SceneEventBus.Emit(new DialogueEvent(dialogue.Select(text => new DialogueItem(speakerName, text))));
        }

        private void OnDestroy()
        {
            _unsubscribe?.Invoke();
        }
    }
}