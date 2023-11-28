using System;
using Game.Src.EventBusModule;
using UnityEngine;
using UnityEngine.Events;

namespace Game.DialogueSystem
{
    public class DialogueEventHandler : MonoBehaviour
    {
        public UnityEvent OnDialogueStarted;
        public UnityEvent OnDialogueEnded;
        
        private Action _unsubscribe;
        
        private void OnEnable()
        {
            var unsub1 = SceneEventBus.Subscribe<DialogueEvent>(HandleDialogueStarted);
            var unsub2 = SceneEventBus.Subscribe<EndDialogueEvent>(HandleDialogueEnded);
            
            _unsubscribe = () =>
            {
                unsub1();
                unsub2();
            };
        }
        
        private void HandleDialogueStarted(DialogueEvent obj)
        {
            OnDialogueStarted?.Invoke();
        }
        
        private void HandleDialogueEnded(EndDialogueEvent obj)
        {
            OnDialogueEnded?.Invoke();
        }
        
        private void OnDisable()
        {
            _unsubscribe();
        }
    }
}