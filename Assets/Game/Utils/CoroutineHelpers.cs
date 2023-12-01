using System;
using System.Collections;
using UnityEngine;

namespace Game.Utils
{
    public static class CoroutineHelpers
    {
        public static IEnumerator DelayedAction(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }
        
        public static IEnumerator AnimatedAction(float duration, Action<float> callback)
        {
            var time = 0f;
            while (time < duration)
            {
                callback(time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            callback(1f);
        }
        
        public static IEnumerator AnimateUIElement(RectTransform rectTransform, Vector2 targetPosition, float duration)
        {
            var startPosition = rectTransform.anchoredPosition;
            yield return AnimatedAction(duration, t =>
            {
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            });
        }
    }
}