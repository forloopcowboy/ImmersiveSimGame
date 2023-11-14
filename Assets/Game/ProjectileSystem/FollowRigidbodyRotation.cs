using UnityEngine;

namespace Game.ProjectileSystem
{
    public class FollowRigidbodyRotation : MonoBehaviour
    {
        public float rotationSpeed = 5.0f; // Adjust this value to control the rotation speed
        public float minSpeedThreshold = 0.1f; // Adjust this value to set the minimum speed threshold for rotation

        public Transform targetTransform; // Set this in the Inspector. If none is provided, it defaults to the script's transform.

        private Rigidbody rb;

        private void Start()
        {
            // Get the Rigidbody component
            rb = GetComponent<Rigidbody>();

            // If no target transform is provided, use the script's transform
            if (targetTransform == null)
            {
                targetTransform = transform;
            }

            // Make sure the Rigidbody has a velocity
            if (rb == null)
            {
                Debug.LogError("Rigidbody component not found on the GameObject.");
            }
        }

        private void Update()
        {
            // Check if the Rigidbody component is available
            if (rb != null)
            {
                // Get the velocity and speed
                Vector3 velocity = rb.velocity;
                float speed = velocity.magnitude;

                // Check if the speed is above the threshold
                if (speed > minSpeedThreshold)
                {
                    // Calculate the rotation angle based on the velocity
                    Quaternion targetRotation = Quaternion.LookRotation(velocity, Vector3.up);

                    // Smoothly rotate the target transform towards the target rotation
                    targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

}