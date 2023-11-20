using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Src.EventBusModule;
using Game.Utils;
using PlasticGui.WorkspaceWindow.BrowseRepository;
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
        
        public int charactersPerSecond = 25;
        
        // Event bus subscription
        private Action _unsubscribe;
        private string _currentDialogueText;
        private bool _isAnimating = false;
        private Coroutine _renderDialogueLettersCoroutine;
        
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
            // If currently rendering dialogue, stop coroutine and show full text. Next event will trigger next dialogue.
            if (_renderDialogueLettersCoroutine != null)
            {
                StopCoroutine(_renderDialogueLettersCoroutine);
                dialogueText.text = _currentDialogueText;
                _renderDialogueLettersCoroutine = null;
                _isAnimating = false;

                return;
            }
            
            // if no dialogue left, hide dialogue box
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
            
            _currentDialogueText = dialogueItem.text;
            _renderDialogueLettersCoroutine = StartCoroutine(RenderDialogueLetters());
        }

        private IEnumerator RenderDialogueLetters()
        {
            _isAnimating = true;
            dialogueText.text = "";
            foreach (var letter in _currentDialogueText)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(1f/charactersPerSecond);
            }
            _isAnimating = false;
            _renderDialogueLettersCoroutine = null;
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