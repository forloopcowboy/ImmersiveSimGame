using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utils
{
    public class VisibilityChecker : MonoBehaviour
    {
        public Transform objectToCheck;
        public LayerMask raycastLayerMask = ~0;
        public float offsetPercentage = .55f;
        
        public UnityEvent onIsVisible;
        public UnityEvent onIsNotVisible;
        
        private Camera _mainCamera;

        private void Start()
        {
            objectToCheck ??= transform;
            _mainCamera ??= Camera.main;

            StartCoroutine(UpdateVisibility());
        }

        // Check if the object is visible from the Camera.main without obstructions
        public static bool IsVisibleFromCamera(Transform objectTransform, Camera mainCamera, LayerMask raycastLayer, float offsetPercentage = .75f)
        {
            if (objectTransform == null)
            {
                Debug.LogError("Transform is null");
                return false;
            }

            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found");
                return false;
            }

            // Calculate the object's position in viewport coordinates
            // Calculate the object's position in viewport coordinates
            // Calculate the object's position in viewport coordinates
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(objectTransform.position);

            // Check if the object is within the camera's view frustum
            if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1 || viewportPoint.z < 0)
            {
                // Object is outside the camera's view frustum
                return false;
            }

            // Check for obstructions using rays from the camera through each corner of the bounding box
            Bounds objectBounds = GetBoundingBox(objectTransform);

            Vector3[] corners = new Vector3[9];
            corners[0] = Vector3.Lerp(objectBounds.min, objectBounds.center, offsetPercentage);
            corners[1] = Vector3.Lerp(new Vector3(objectBounds.min.x, objectBounds.min.y, objectBounds.max.z), objectBounds.center, offsetPercentage);
            corners[2] = Vector3.Lerp(new Vector3(objectBounds.min.x, objectBounds.max.y, objectBounds.min.z), objectBounds.center, offsetPercentage);
            corners[3] = Vector3.Lerp(new Vector3(objectBounds.min.x, objectBounds.max.y, objectBounds.max.z), objectBounds.center, offsetPercentage);
            corners[4] = Vector3.Lerp(new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.min.z), objectBounds.center, offsetPercentage);
            corners[5] = Vector3.Lerp(new Vector3(objectBounds.max.x, objectBounds.min.y, objectBounds.max.z), objectBounds.center, offsetPercentage);
            corners[6] = Vector3.Lerp(new Vector3(objectBounds.max.x, objectBounds.max.y, objectBounds.min.z), objectBounds.center, offsetPercentage);
            corners[7] = Vector3.Lerp(objectBounds.max, objectBounds.center, offsetPercentage);
            corners[8] = objectBounds.center;

            foreach (Vector3 corner in corners)
            {
                RaycastHit hit;
                if (Physics.Raycast(mainCamera.transform.position, corner - mainCamera.transform.position, out hit, 700f, raycastLayer))
                {
                    // Draw a debug line to visualize the raycast
                    Debug.DrawLine(mainCamera.transform.position, hit.point, hit.transform.GetInstanceID() == objectTransform.GetInstanceID() ? Color.green : Color.red);

                    // If the hit object is not the object we are checking, there is an obstruction
                    if (hit.transform != objectTransform) continue;
                    return true;
                }

                Debug.DrawRay(mainCamera.transform.position, (corner - mainCamera.transform.position).normalized * 700f, Color.red);
            }

            // If no obstructions and within view frustum, the object is visible
            return false;
        }

        // Get the bounding box of the object
        private static Bounds GetBoundingBox(Transform objectTransform)
        {
            Renderer[] renderers = objectTransform.GetComponentsInChildren<Renderer>();
            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        // Example usage:
        IEnumerator UpdateVisibility()
        {
            while (true)
            {
                if (IsVisibleFromCamera(
                    objectToCheck,
                    _mainCamera,
                    raycastLayerMask,
                    offsetPercentage
                    )) onIsVisible?.Invoke();
                
                else onIsNotVisible?.Invoke();
                
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}