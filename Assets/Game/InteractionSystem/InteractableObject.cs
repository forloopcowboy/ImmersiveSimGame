using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.InteractionSystem
{
    public class InteractableObject : SerializedMonoBehaviour
    {
        public bool isInteractable = true;

        [TabGroup("General")] public string shortcut = "E";
        [TabGroup("General")] public string interactionText = "to inspect";
        [TabGroup("General")] public string itemName = "Item";

        [TabGroup("Interaction Events")] public UnityEvent<Interactor, object> OnInteracted;
        [TabGroup("Interaction Events"), UnityEngine.Tooltip("Text to display when user interacts with object. Use $itemName to refer to itemName.")] public string InteractNotification = "";
        [TabGroup("Interaction Events"), UnityEngine.Tooltip("Text to display when user interacts with object. Use $itemName to refer to itemName.")] public string InteractDialogue = "";

        public string GetInteractNotification()
        {
            return InteractNotification.Replace("$itemName", itemName);
        }
        
        public string GetInteractDialogue()
        {
            return InteractDialogue.Replace("$itemName", itemName);
        }
        
        protected virtual void OnInteract(Interactor interactor, object input)
        { }
        
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