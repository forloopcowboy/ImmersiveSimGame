using UnityEngine;
using UnityEngine.Events;

namespace Game.Utils
{
    public class DoInSeconds : MonoBehaviour
    {
        [SerializeField]
        private float waitTime = 1f;
        Coroutine _coroutine;
        
        public UnityEvent action; 

        private void Start()
        {
            _coroutine = StartCoroutine(CoroutineHelpers.DelayedAction(waitTime, () =>
            {
                action?.Invoke();
            }));
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }
    }
}