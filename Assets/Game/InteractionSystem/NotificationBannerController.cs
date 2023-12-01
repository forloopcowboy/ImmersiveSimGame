using System;
using System.Collections;
using Game.Utils;
using PlasticGui.Help;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.InteractionSystem
{
    public class NotificationBannerController : MonoBehaviour
    {
        public float fadeInDuration = 0.5f;
        public float fadeOutDuration = 0.5f;
        public float stayDuration = 6f;

        public AnimationCurve fadeCurve;
        
        private RectTransform rectTransform;
        private TMP_Text text;
        private LayoutElement layoutElement;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            text = GetComponentInChildren<TMP_Text>();
            layoutElement = GetComponent<LayoutElement>();
            
            StartCoroutine(Show(fadeInDuration));
            StartCoroutine(CoroutineHelpers.DelayedAction(fadeInDuration + stayDuration, () => StartCoroutine(Hide(fadeOutDuration))));
        }

        private IEnumerator Show(float duration)
        {
            // Sanity check
            if (duration <= 0.0f)
            {
                Debug.LogError("Duration must be greater than 0.0f");
                yield break;
            }
            
            // Wait for a frame to allow the layout to update
            yield return null;
            yield return null;

            float startHeight = rectTransform.rect.height;
            float endHeight = text.preferredHeight + 10f;
            float startWidth = rectTransform.rect.width;
            float endWidth = text.preferredWidth + 10f;
            
            float time = 0.0f;
            while (time < duration)
            {
                // Adding easing to the animation makes the animation much more fluid, compared to
                // a standard linear interpolation; see the repository below for the implementation
                layoutElement.preferredHeight = Mathf.Lerp(startHeight, endHeight, fadeCurve.Evaluate(time / duration));
                layoutElement.preferredWidth = Mathf.Lerp(startWidth, endWidth, fadeCurve.Evaluate(time / duration));

                yield return null;
                time += Time.deltaTime;
            }
            
            layoutElement.preferredHeight = endHeight;
            layoutElement.preferredWidth = endWidth;
        }
        
        private IEnumerator Hide(float duration)
        {
            // Sanity check
            if (duration <= 0.0f)
            {
                Debug.LogError("Duration must be greater than 0.0f");
                yield break;
            }

            float startHeight = text.preferredHeight;
            float endHeight = 0;
            float startWidth = text.preferredWidth;
            float endWidth = 0;
            
            float time = 0.0f;
            while (time < duration)
            {
                // Adding easing to the animation makes the animation much more fluid, compared to
                // a standard linear interpolation; see the repository below for the implementation
                layoutElement.preferredHeight = Mathf.Lerp(startHeight, endHeight, fadeCurve.Evaluate(time / duration));
                layoutElement.preferredWidth = Mathf.Lerp(startWidth, endWidth, fadeCurve.Evaluate(time / duration));

                yield return null;
                time += Time.deltaTime;
            }
            
            layoutElement.preferredHeight = endHeight;
            layoutElement.preferredWidth = endWidth;
            
            Destroy(gameObject, 1);
        }
    }
}