using UnityEngine;

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
        public bool animateOnEnable = false;
        public DisappearMode disappearMode = DisappearMode.Destroy;

        private void OnEnable()
        {
            transform.localScale = new Vector3(1,1,1);
            
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
            float timer = 0f;

            while (timer < duration)
            {
                float scale = disappearCurve.Evaluate(timer / duration);
                transform.localScale = new Vector3(scale, scale, scale);

                timer += Time.deltaTime;
                yield return null;
            }

            // Ensure the final scale is set
            transform.localScale = Vector3.zero;

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
        }
    }

}