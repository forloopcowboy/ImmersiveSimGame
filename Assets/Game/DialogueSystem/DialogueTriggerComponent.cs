using System;
using System.Linq;
using Game.Src.EventBusModule;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.DialogueSystem
{
    [RequireComponent(typeof(Collider))]
    public class DialogueTriggerComponent : SerializedMonoBehaviour
    {
        [InfoBox("Resets once triggered")]
        public bool shouldTrigger = true;
        
        [InfoBox("Dialogue to trigger")]
        public string speakerName = "???";
        public string[] dialogue;

        private void OnTriggerEnter(Collider other)
        {
            if (!shouldTrigger) return;
            
            SceneEventBus.Emit(new DialogueEvent(dialogue.Select(text => new DialogueItem(speakerName, text))));
            shouldTrigger = false;
        }
    }
}