using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.SaveUtility;
using Game.Src.EventBusModule;
using Game.Utils;
using PlasticGui.WorkspaceWindow.BrowseRepository;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.DialogueSystem
{
    public class DialogueSystem : SingletonMonoBehaviour<DialogueSystem>
    {
        [TabGroup("Dialogue"), ShowInInspector]
        public Queue<DialogueItem> dialogueQueue = new();
        [Required, TabGroup("Settings")]
        public Transform dialogueBox;
        [Required, TabGroup("Settings")]
        public TMP_Text speakerText;
        [Required, TabGroup("Settings")]
        public TMP_Text dialogueText;

        [TabGroup("Dialogue")]
        public int charactersPerSecond = 25;
        [TabGroup("Dialogue")]
        public float timeScaleOnDialogue = 0.16f;

        [TabGroup("Events")]
        public UnityEvent OnDialogueShow;
        [TabGroup("Events")]
        public UnityEvent OnDialogueHide;
        
        // Event bus subscription
        private Action _unsubscribe;
        private DialogueItem _currentDialogueItem;
        private bool _isAnimating = false;
        private Coroutine _renderDialogueLettersCoroutine;
        
        private void Start()
        {
            ShowNextDialogue();
            InitializeEventBus();
        }

        private void OnEnable()
        {
            InitializeEventBus();

            // if dialogue was interrupted, skip animation
            if (_isAnimating)
            {
                if (_renderDialogueLettersCoroutine != null)
                {
                    StopCoroutine(_renderDialogueLettersCoroutine);
                }
                
                DisplayFullDialogue();
            }
        }
        
        public static bool CanSkipDialogue()
        {
            return Singleton.dialogueQueue.Count > 0 && Singleton.dialogueQueue.Peek().skippable;
        }

        private void InitializeEventBus()
        {
            if (_unsubscribe != null)
            {
                _unsubscribe();
            }
            
            var unsub1 = SceneEventBus.Subscribe<DialogueEvent>(OnDialogueEvent);
            var unsub2 = SceneEventBus.Subscribe<NextDialogueEvent>(ShowNextDialogue);
            var unsub3 = SceneEventBus.Subscribe<SkipDialogueEvent>(SkipDialogue);
            var unsub4 = SceneEventBus.Subscribe<EndDialogueEvent>(OnDialogueEnded);
            
            _unsubscribe = () =>
            {
                unsub1();
                unsub2();
                unsub3();
                unsub4();
            };
        }

        private void OnDialogueEnded(EndDialogueEvent obj)
        {
            Time.timeScale = 1f;
            // update physics time scale
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        private void SkipDialogue(SkipDialogueEvent obj = null)
        {
            // if next dialogue is skippable, skip it and all following skippable dialogues
            while (dialogueQueue.Count > 0 && dialogueQueue.Peek().skippable)
            {
                dialogueQueue.Dequeue();
            }
            ShowNextDialogue();
            ShowNextDialogue();
        }

        private void OnDialogueEvent(DialogueEvent obj)
        {
            Debug.Log($"Dialogue event received: {obj.dialogueItems.Count()} DialogueItems added to queue. Time scale is now {timeScaleOnDialogue}");
            Time.timeScale = timeScaleOnDialogue;
            // update physics time scale
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            
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
                _renderDialogueLettersCoroutine = null;
            }
            
            if (_isAnimating)
            {
                DisplayFullDialogue();
                return;
            }
            
            // if no dialogue left, hide dialogue box
            if (dialogueQueue.Count == 0)
            {
                if (dialogueBox.gameObject.activeSelf)
                {
                    SceneEventBus.Emit(new EndDialogueEvent());
                    dialogueBox.gameObject.SetActive(false);
                    OnDialogueHide?.Invoke();
                }
                
                return;
            }
            
            var dialogueItem = dialogueQueue.Dequeue();
            
            // emit unity event
            if (!dialogueBox.gameObject.activeSelf)
            {
                OnDialogueShow?.Invoke();
                dialogueBox.gameObject.SetActive(true);
            }
            
            speakerText.text = dialogueItem.speakerName;
            
            _currentDialogueItem = dialogueItem;
            _renderDialogueLettersCoroutine = StartCoroutine(RenderDialogueLetters());
        }

        private void DisplayFullDialogue()
        {
            dialogueText.text = _currentDialogueItem.text;
            _isAnimating = false;
            
            _currentDialogueItem.onPlayed?.Invoke();
            
            // emit events
            var npcComponent = _currentDialogueItem.speakerAvatar != null ? _currentDialogueItem.speakerAvatar.GetComponentInChildren<ISerializedActor>() : null;
            if (npcComponent != null && !String.IsNullOrEmpty(npcComponent.GetIdentifier()))
            {
                foreach (var npcEvent in _currentDialogueItem.EmitNpcEvents)
                {
                    GlobalGameState.State.RecordNpcEvent(
                        npcComponent.GetIdentifier(),
                        new SerializedEvent("DialogueEvent", npcEvent)
                    );
                }
            }
        }

        private IEnumerator RenderDialogueLetters()
        {
            _isAnimating = true;
            dialogueText.text = "";
            foreach (var letter in _currentDialogueItem.text)
            {
                if (!_isAnimating) break;
                dialogueText.text += letter;
                
                var normalInterval = 1f/ (charactersPerSecond / Time.timeScale);

                if (letter is ' ' or '.' or ',' or '!' or '?')
                    yield return new WaitForSeconds(normalInterval * 2);
                yield return new WaitForSeconds(normalInterval);
            }
            
            DisplayFullDialogue();
        }

        private void OnDisable()
        {
            _unsubscribe?.Invoke();
            _unsubscribe = null;
        }
    }
    
    public class DialogueItem
    {
        public string speakerName;
        public string dialogId;
        public string text;
        public bool skippable = true;
        public GameObject speakerAvatar;
        
        public List<string> EmitNpcEvents = new List<string>();
        
        public UnityEvent onPlayed = new UnityEvent();

        public DialogueItem(string speakerName, string text, bool skippable = true, GameObject speakerAvatar = null)
        {
            this.speakerName = speakerName;
            this.text = text;
            this.speakerAvatar = speakerAvatar;
        }
    }

    /// <summary>
    /// Emitted by player when dialogue ends.
    /// </summary>
    public class EndDialogueEvent { }
    
    public class NextDialogueEvent { }
    
    public class SkipDialogueEvent { }

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