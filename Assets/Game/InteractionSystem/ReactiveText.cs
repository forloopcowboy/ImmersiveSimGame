using System;
using Game.Src.EventBusModule;
using Game.Utils;
using Sirenix.OdinInspector;
using TMPro;

namespace Game.InteractionSystem
{
    /// <summary>
    /// Text that changes value based on SceneEventBus events.
    /// See
    /// </summary>
    public class ReactiveText : SerializedMonoBehaviour
    {
        [Required] public StringConstant id;
        [Required] public TMP_Text textComponent;
        
        public string initialValue;

        private Action unsubscribe;
        
        private void OnValidate()
        {
            if (textComponent == null)
                textComponent = GetComponent<TMP_Text>() ?? GetComponentInChildren<TMP_Text>();
        }

        private void OnEnable()
        {
            unsubscribe = SceneEventBus.Subscribe<ReactiveTextChangeValueEvent>(OnReactiveTextChangeValueEvent);
        }

        private void OnDisable()
        {
            unsubscribe?.Invoke();
        }

        private void OnReactiveTextChangeValueEvent(ReactiveTextChangeValueEvent e)
        {
            if (e.id == id)
            {
                textComponent.text = e.value;
            }
        }
    }

    public class ReactiveTextChangeValueEvent
    {
        public string id;
        public string value;
    }
}