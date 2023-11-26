using Game.HealthSystem;
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
        public float rotationSpeed = 65f;
        public float dampingFactor = 5f;
        public float stopDistance = 0.01f;

        public UnityEvent<GameObject> onGrab;
        public UnityEvent<GameObject> onThrow;
        
        public void Grab(GameObject targetGrab)
        {
            if (isGrabbed) return;
            
            onGrab?.Invoke(targetGrab);

            isGrabbed = true;
            grabObject = targetGrab;
        }
        
        public void Release()
        {
            if (!isGrabbed) return;
            isGrabbed = false;
            if (grabObject.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

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
                rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
            }
            
            if (grabObject.TryGetComponent(out GrabbableObject grabbableObject))
            {
                grabbableObject.OnThrow.Invoke();
            }
            else Debug.LogWarning("GrabbableObject does not have OnThrow event");
            
            if (grabObject.TryGetComponent(out DamageComponent damageComponent))
            {
                damageComponent.enabled = true;
                damageComponent.damageSource = gameObject;
            }
            else Debug.LogWarning("GrabbableObject does not have DamageComponent");
            
            onThrow?.Invoke(grabObject);
            Release();
        }

        public void FixedUpdate()
        {
            // if is grabbed use object physics to move if availbe
            if (isGrabbed)
            {
                if (grabObject.TryGetComponent(out Rigidbody rb))
                {
                    rb.useGravity = false;
                    AttractToGrabNode(transform, rb, attractionForce, rotationSpeed, dampingFactor, stopDistance);
                }
                else
                {
                    grabObject.transform.position = transform.position;
                    grabObject.transform.rotation = transform.rotation;
                }
            }
        }
        
        public static void AttractToGrabNode(Transform grabNode, Rigidbody rb, float attractionForce, float rotationSpeed, float dampingFactor, float stopDistance = 0.1f)
        {
            if (grabNode == null || rb == null)
            {
                return;
            }

            // If distance is small enough, stop the object from moving
            if (Vector3.Distance(grabNode.position, rb.position) < stopDistance)
            {
                // lerp speed to zero
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * dampingFactor);
                rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * dampingFactor);
                return;
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
        }
    }
}