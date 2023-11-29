using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem
{
    public abstract class InteractableObject : SerializedMonoBehaviour
    {
        public bool isInteractable = true;

        [TabGroup("General")] public string shortcut = "E";
        [TabGroup("General")] public string interactionText = "to inspect";
        [TabGroup("General")] public string itemName = "Item";

        public UnityEvent<Interactor, object> OnInteracted;

        protected abstract void OnInteract(Interactor interactor, object input);
        
        public void Interact(Interactor interactor, object input)
        {
            if (isInteractable)
            {
                OnInteracted?.Invoke(interactor, input);
                OnInteract(interactor, input);
            }
        }
    }
}