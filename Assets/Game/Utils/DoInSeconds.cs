using Game.Src.EventBusModule;
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
            // launch coroutine from event bus to still destroy it in case this object is disabled in the pooling process.
            _coroutine = SceneEventBus.Singleton.StartCoroutine(CoroutineHelpers.DelayedAction(waitTime, () =>
            {
                action?.Invoke();
            }));
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}