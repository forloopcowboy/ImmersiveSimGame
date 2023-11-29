using Game.InteractionSystem;
using UnityEngine.Events;

namespace Game.GrabSystem
{
    public class GrabbableObject : InteractableObject
    {
        public UnityEvent OnThrow;
        
        protected override void OnInteract(Interactor interactor, object input)
        {
            if (input is GrabNode grabNode)
            {
                grabNode.Grab(gameObject);
            }
            else throw new System.Exception("Could not interact with GrabbableObject: Input is not of type GrabNode");
        }
    }
}