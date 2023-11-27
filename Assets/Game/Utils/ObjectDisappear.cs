using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Game.Utils
{
    public enum DisappearMode
    {
        Destroy,
        Disable
    }
    
    public class ObjectDisappear : MonoBehaviour
    {
        public AnimationCurve disappearCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        public float duration = 1.0f;
        public float delay = 0f;
        public bool animateOnEnable = false;
        public DisappearMode disappearMode = DisappearMode.Destroy;
        
        public UnityEvent OnDisappear;

        // Handle decals
        private DecalProjector _decalProjector;
        
        // Handle initial scale
        private float initialScale;

        private void Awake()
        {
            initialScale = transform.localScale.magnitude;
        }

        private void OnEnable()
        {
            SetScale(initialScale);
            _decalProjector = GetComponentInChildren<DecalProjector>();

            if (animateOnEnable)
            {
                Disappear();
            }
        }

        public void Disappear()
        {
            StartCoroutine(DisappearCoroutine());
        }

        private System.Collections.IEnumerator DisappearCoroutine()
        {
            if (delay > 0.000001f)
            {
                yield return new WaitForSeconds(delay);
            }
            
            float timer = 0f;

            while (timer < duration)
            {
                float scale = disappearCurve.Evaluate(timer / duration);
                SetScale(scale * initialScale);

                timer += Time.deltaTime;
                yield return null;
            }

            // Ensure the final scale is set
            SetScale(0f);

            // Decide whether to destroy or disable the object
            if (disappearMode == DisappearMode.Destroy)
            {
                Debug.Log($"{name} Destroyed");
                Destroy(gameObject);
            }
            else if (disappearMode == DisappearMode.Disable)
            {
                Debug.Log($"{name} Disabled");
                gameObject.SetActive(false);
            }
            
            OnDisappear?.Invoke();
        }

        private void SetScale(float scale)
        {
            if (_decalProjector != null)
                _decalProjector.size = new Vector3(scale, scale, _decalProjector.size.z);
            else transform.localScale = new Vector3(scale, scale, scale);
        }
    }

}