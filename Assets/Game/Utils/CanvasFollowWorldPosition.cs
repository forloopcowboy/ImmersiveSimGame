using UnityEngine;

namespace Game.Utils
{
    [ExecuteAlways]
    public class CanvasFollowWorldPosition : MonoBehaviour
    {
        [Header("Tweaks")] 
        public Transform lookAt;
        public Vector3 offset;
        public Vector3 lookAtOffset = Vector3.zero;
        public Canvas canvas;

        // //

        private Camera cam;
        private RectTransform rt;
        
        public bool lookAtIsNotVisible => lookAt && Vector3.Dot(lookAt.position - cam.transform.position, cam.transform.forward) < 0;
        public bool lookAtIsVisible => !lookAtIsNotVisible;

        private void Start()
        {
            cam = canvas.worldCamera ? canvas.worldCamera : Camera.main;
            rt = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (!lookAt) return;

            var cameraTransform = cam.transform;

            if (lookAtIsNotVisible) return;
            
            Vector2 adjustedPosition = cam.WorldToScreenPoint(lookAt.position + lookAtOffset) + offset;

            var canvasRT = canvas.GetComponent<RectTransform>();
            var rect = canvasRT.rect;
            adjustedPosition.x *= rect.width / (float)cam.pixelWidth;
            adjustedPosition.y *= rect.height / (float)cam.pixelHeight;
 
            // set it
            rt.anchoredPosition = adjustedPosition - canvasRT.sizeDelta / 2f;
        }
    }
}