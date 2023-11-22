using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Utils.EventBusModule
{
    public class DoOnTrigger : SerializedMonoBehaviour
    {
        [InfoBox("Number of collisions to wait for before executing the action.")]
        public int count = 1;
        public int iterations = 1;
        
        [InfoBox("Action to execute when the number of collisions is reached.")]
        public UnityEngine.Events.UnityEvent action;
        [FormerlySerializedAs("onTrigger")] public UnityEngine.Events.UnityEvent<Collider> onTriggerEnter;
        public UnityEngine.Events.UnityEvent<Collider> onTriggerStay;
        [TabGroup("Debug")] [SerializeField] [ReadOnly] [InfoBox("Current collision count.")]
        private int _currentCount = 0;
        [TabGroup("Debug")] [SerializeField] [ReadOnly] [InfoBox("Current iteration count.")]
        private int _iteration = 1;

        private void OnEnable()
        {
            _currentCount = 0;
            _iteration = 1;
        }

        private void OnTriggerEnter(Collider other)
        {
            _currentCount++;
            if (_currentCount == count * _iteration && _iteration <= iterations)
            {
                action?.Invoke();
                onTriggerEnter?.Invoke(other);
                _iteration++;
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }

        public void DestroySelf(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}