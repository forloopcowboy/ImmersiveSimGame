using System;
using System.Collections.Generic;
using System.Linq;
using Game.Src.EventBusModule;
using Game.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.DialogueSystem
{
    public class DialoguePlayer : SingletonMonoBehaviour<DialoguePlayer>
    {
        public Queue<DialogueItem> dialogueQueue = new();
        [Required]
        public Transform dialogueBox;
        [Required]
        public TMP_Text speakerText;
        [Required]
        public TMP_Text dialogueText;
        
        // Event bus subscription
        private Action _unsubscribe;
        
        private void Start()
        {
            ShowNextDialogue();
            
            var unsub1 = SceneEventBus.Subscribe<DialogueEvent>(OnDialogueEvent);
            var unsub3 = SceneEventBus.Subscribe<NextDialogueEvent>(ShowNextDialogue);
            
            _unsubscribe = () =>
            {
                unsub1();
                unsub3();
            };
        }

        private void OnDialogueEvent(DialogueEvent obj)
        {
            Debug.Log($"Dialogue event received: {obj.dialogueItems.Count()} DialogueItems added to queue");
            EnqueueDialogueItems(obj.dialogueItems);
            ShowNextDialogue();
        }

        /// <summary>
        /// Enqueues dialogue items.
        /// </summary>
        public void EnqueueDialogueItems(IEnumerable<DialogueItem> dialogueItems)
        {
            foreach (var dialogueItem in dialogueItems)
            {
                dialogueQueue.Enqueue(dialogueItem);
            }
        }

        /// <summary>
        /// Dequeues and shows dialogue. If at queue end, hides dialogue.
        /// </summary>
        public void ShowNextDialogue(NextDialogueEvent obj = null)
        {
            if (dialogueQueue.Count == 0)
            {
                if (dialogueBox.gameObject.activeSelf)
                    SceneEventBus.Emit(new EndDialogueEvent());
                dialogueBox.gameObject.SetActive(false);
                return;
            }
            
            var dialogueItem = dialogueQueue.Dequeue();
            dialogueBox.gameObject.SetActive(true);
            speakerText.text = dialogueItem.speaker;
            dialogueText.text = dialogueItem.text;
        }

        private void OnDisable()
        {
            _unsubscribe?.Invoke();
        }
    }
    
    public class DialogueItem
    {
        public string speaker;
        public string text;

        public DialogueItem(string speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }
    }

    /// <summary>
    /// Emitted by player when dialogue ends.
    /// </summary>
    public class EndDialogueEvent { }
    
    public class NextDialogueEvent { }

    public class DialogueEvent
    {
        public IEnumerable<DialogueItem> dialogueItems;
        
        public DialogueEvent(IEnumerable<DialogueItem> dialogueItems)
        {
            this.dialogueItems = dialogueItems;
        }
        
        public DialogueEvent(params DialogueItem[] dialogueItems)
        {
            this.dialogueItems = dialogueItems;
        }
    }
}