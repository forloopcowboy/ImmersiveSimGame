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

        public abstract void Interact(dynamic input);
    }
}