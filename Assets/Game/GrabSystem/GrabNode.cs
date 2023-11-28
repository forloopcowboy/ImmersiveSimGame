using Game.HealthSystem;
using Game.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Game.GrabSystem
{
    public class GrabNode : MonoBehaviour
    {
        public GameObject grabObject;
        public bool isGrabbed;
        public float throwForce = 10f;
        public float attractionForce = 100f;
        public float dampingFactor = 5f;
        public float stopDistance = 0.01f;
        public float dropDistance = 1.5f;

        // Only drop based on distance if the object has been grabbed
        private bool _hasGrabbed;
        
        public void Grab(GameObject targetGrab)
        {
            if (isGrabbed) return;
            
            GenericCollisionHandler.SetCollisionHandling(transform.root.gameObject, targetGrab, true);

            isGrabbed = true;
            grabObject = targetGrab;
        }
        
        public void Release(bool stopIgnoringCollisions = true)
        {
            if (!isGrabbed) return;
            isGrabbed = false;
            _hasGrabbed = false;
            
            if (grabObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
            
            if (stopIgnoringCollisions) 
                GenericCollisionHandler.SetCollisionHandling(transform.root.gameObject, grabObject, false);

            grabObject = null;
        }

        public void Throw()
        {
            if (!isGrabbed) return;
            
            // throw using object physics if available
            if (grabObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.velocity = Vector3.zero;
                rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
            }
            
            // Ignore self for a brief moment to avoid collision with grab node
            GenericCollisionHandler.IgnoreCollisionsForSeconds(transform.root.gameObject, grabObject, .25f);
            
            if (grabObject.TryGetComponent(out GrabbableObject grabbableObject))
            {
                grabbableObject.OnThrow.Invoke();
            }
            else Debug.LogWarning("GrabbableObject does not have OnThrow event");
            
            if (grabObject.TryGetComponent(out DamageComponent damageComponent))
            {
                damageComponent.enabled = true;
                damageComponent.damageSource = transform.root.gameObject;
            }
            else Debug.LogWarning("GrabbableObject does not have DamageComponent");
            
            // Let the coroutine re-enable collisions to avoid self damage
            Release(false);
        }

        public void FixedUpdate()
        {
            // if is grabbed use object physics to move if availbe
            if (isGrabbed)
            {
                if (grabObject.TryGetComponent(out Rigidbody rb))
                {
                    rb.useGravity = false;
                    var reachedNode = AttractToGrabNode(transform, rb, attractionForce, dampingFactor, stopDistance);

                    // only if we reached node once, we set it to true, even if it leaves the stop distance later
                    if (reachedNode) _hasGrabbed = true;
                    
                    // if already grabbed once, but then left the stop distance, drop the object
                    if (_hasGrabbed && Vector3.Distance(transform.position, rb.position) > dropDistance)
                        Release(); // Immediately re-enable collisions
                }
                else
                {
                    grabObject.transform.position = transform.position;
                    grabObject.transform.rotation = transform.rotation;
                }
            }
        }
        
        
        /// <returns>True if the rigidbody is close enough to grab node based on stop distance.</returns>
        public static bool AttractToGrabNode(Transform grabNode, Rigidbody rb, float attractionForce, float dampingFactor, float stopDistance = 0.1f)
        {
            if (grabNode == null || rb == null)
            {
                return false;
            }

            // If distance is small enough, stop the object from moving
            if (Vector3.Distance(grabNode.position, rb.position) < stopDistance)
            {
                // lerp speed to zero
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * dampingFactor);
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * dampingFactor);
                return true;
            }

            rb.MoveRotation(grabNode.rotation);

            // Calculate the direction from the current position to the grabNode
            Vector3 directionToGrabNode = grabNode.position - rb.position;

            // Calculate the attraction force
            Vector3 attractionForceVector = directionToGrabNode.normalized * attractionForce;

            // Calculate the damping force to prevent oscillations
            Vector3 dampingForce = -rb.velocity * dampingFactor;

            // Calculate the total force
            Vector3 totalForce = attractionForceVector + dampingForce;

            // Apply the force to the Rigidbody
            rb.AddForce(totalForce);

            return false;
        }
    }
}