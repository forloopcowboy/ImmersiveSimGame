using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.InteractionSystem
{
    public enum InteractionMethod
    {
        TRIGGER,
        RAYCAST,
        SPHERECAST
    }
    
    public class Interactor : SerializedMonoBehaviour
    {
        [TabGroup("Interaction")]
        public InteractionMethod InteractionMethod = InteractionMethod.TRIGGER;
        [TabGroup("Interaction"), HideIf("IsTrigger")] public bool useCameraForward = true;
        [TabGroup("Interaction"), HideIf("IsTrigger")] public float raycastRange = 1.7f;
        [TabGroup("Interaction"), HideIf("IsTrigger"), ShowIf("IsSpherecast")] public float spherecastRadius = .5f;
        [TabGroup("Interaction"), HideIf("IsTrigger"), SerializeField] private LayerMask raycastMask = ~0;
        
        public bool IsTrigger => InteractionMethod == InteractionMethod.TRIGGER;
        public bool IsSpherecast => InteractionMethod == InteractionMethod.SPHERECAST;
        
        [ReadOnly, ShowInInspector, TabGroup("Interaction Runtime")]
        private HashSet<InteractableObject> _inInteractRange = new();
        [ReadOnly, ShowInInspector, TabGroup("Interaction Runtime")]
        private Stack<InteractableObject> _interactionQueue = new();
        private Camera _camera;

        public bool TryPeekInteractionQueue<T>(out T item) where T : InteractableObject
        {
            if (_interactionQueue.TryPeek(out var interactable) && interactable is T result)
            {
                item = result;
                return item != null;
            }

            item = null;
            return false;
        }
        
        public bool TryToInteract<T, TInput>(out T item, TInput input = default) where T : InteractableObject
        {
            if (_interactionQueue.Count == 0)
            {
                item = null;
                return false;
            }

            if (TryPeekInteractionQueue(out item) && _interactionQueue.TryPop(out var interactable) && interactable is T result)
            {
                item = result;
                item.Interact(input);
                _inInteractRange.Remove(item);
                
                return item != null;
            }

            item = null;
            return false;
        }

        private void Start()
        {
            if (!_camera) _camera = Camera.main;
            
            StartCoroutine(UpdateRaycast());
        }

        private IEnumerator UpdateRaycast()
        {
            while (true)
            {
                RaySphereCast();
                yield return new WaitForSeconds(0.1f);
            }
        }

        // keep track of previous hit in frame
        bool hit;
        RaycastHit hitInfo;
        
        private void RaySphereCast()
        {
            if (InteractionMethod == InteractionMethod.RAYCAST || InteractionMethod == InteractionMethod.SPHERECAST)
            {
                var previousHitInfo = hitInfo;
                
                _camera = Camera.main;
                Ray ray = useCameraForward ? _camera.ViewportPointToRay(new Vector2(0.5f, 0.5f)) : new Ray(transform.position, transform.forward);

                if (InteractionMethod == InteractionMethod.RAYCAST)
                {
                    hit = Physics.Raycast(ray, out hitInfo, raycastRange, raycastMask);
                }
                else
                {
                    hit = Physics.SphereCast(ray, spherecastRadius, out hitInfo, raycastRange, raycastMask);
                }
                
                var interactableObject = hit ? GetInteractableObject(hitInfo.collider) : null;
                var didHitInteractable = hit && interactableObject != null;
                Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastRange, didHitInteractable ? Color.green : Color.red, 0.25f);
                if (didHitInteractable) Debug.DrawLine(ray.origin, hitInfo.point, Color.blue, 0.25f);

                if (previousHitInfo.collider != hitInfo.collider && previousHitInfo.collider != null)
                {
                    OnColliderOutOfRange(previousHitInfo.collider);
                }
                
                if (hit)
                {
                    OnColliderInRange(hitInfo.collider);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (InteractionMethod == InteractionMethod.TRIGGER) OnColliderInRange(other);
        }
        
        public InteractableObject GetInteractableObject(Collider other)
        {
            if (other == null) return null;
            
            var item = other.GetComponent<InteractableObject>();
            if (!item) item = other.GetComponentInParent<InteractableObject>();
            if (!item) item = other.GetComponentInChildren<InteractableObject>();
            return item;
        }

        private void OnColliderInRange(Collider other)
        {
            var item = GetInteractableObject(other);
            
            if (item == null) return;
            if (_inInteractRange.Contains(item)) return;

            _inInteractRange.Add(item);
            _interactionQueue.Push(item);
        }

        private void OnTriggerExit(Collider other)
        {
            if (InteractionMethod == InteractionMethod.TRIGGER) OnColliderOutOfRange(other);
        }

        private void OnColliderOutOfRange(Collider other)
        {
            // remove item from set and stack
            var item = other.GetComponent<InteractableObject>();
            if (!item) item = other.GetComponentInParent<InteractableObject>();
            if (!item) item = other.GetComponentInChildren<InteractableObject>();
            
            if (item == null) return;
            if (!_inInteractRange.Contains(item)) return;

            _inInteractRange.Remove(item);
            _interactionQueue = new Stack<InteractableObject>(_interactionQueue.Where(i => i != item));
        }
    }
}