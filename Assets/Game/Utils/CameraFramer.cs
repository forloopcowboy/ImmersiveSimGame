using UnityEngine;

namespace Game.Utils
{
    public static class CameraFramer
    {
        public static void FrameObject(this Camera cam, GameObject obj)
        {
            // Get the bounds of the GameObject and its children
            Bounds bounds = CalculateBoundsWithChildren(obj);

            // Calculate the distance the camera needs to move to fit the bounds
            float objectSize = bounds.size.magnitude;
            float distance = objectSize / (2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad));

            // Calculate the new position for the camera
            Vector3 targetPosition = bounds.center;
            Vector3 directionToTarget = cam.transform.position - targetPosition;
            Vector3 newPosition = targetPosition + directionToTarget.normalized * distance;

            // Move the camera to the new position
            cam.transform.position = newPosition;
            cam.transform.LookAt(targetPosition);
        }

        public static Bounds CalculateBoundsWithChildren(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            Bounds bounds = new Bounds(renderers[0].bounds.center, Vector3.zero);

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }
    }
}