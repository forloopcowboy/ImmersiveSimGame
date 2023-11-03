using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.InteractionSystem
{
    public abstract class InteractableObject : SerializedMonoBehaviour
    {
        public bool isInteractable = true;

        [TabGroup("General")] public string shortcut = "E";
        [TabGroup("General")] public string interactionText = "to inspect";
        [TabGroup("General")] public string itemName = "Item";
        
        public virtual void Interact(dynamic input)
        {
            Debug.Log("I just wanted" + interactionText + "a little bit of " + name);
        }
    }
}