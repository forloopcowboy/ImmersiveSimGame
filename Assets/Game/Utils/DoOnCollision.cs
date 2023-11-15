using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Utils
{
    public class DoOnCollision : SerializedMonoBehaviour
    {
        [InfoBox("Number of collisions to wait for before executing the action.")]
        public int count = 1;
        public int iterations = 1;
        
        [InfoBox("Action to execute when the number of collisions is reached.")]
        public UnityEngine.Events.UnityEvent action;
        public UnityEngine.Events.UnityEvent onReset;
        public UnityEngine.Events.UnityEvent<Collision> onCollision;

        private int _currentCount = 0;
        private int _iteration = 1;

        private void Start()
        {
            _iteration = iterations;
        }
        
        private void OnEnable()
        {
            _currentCount = 0;
            _iteration = 1;
            
            onReset?.Invoke();
        }

        private void OnCollisionEnter(Collision other)
        {
            _currentCount++;
            if (_currentCount == count * _iteration && _iteration <= iterations)
            {
                action?.Invoke();
                onCollision?.Invoke(other);
                _iteration++;
            }
        }

        public void DestroySelf(float delay)
        {
            Destroy(gameObject, delay);
        }

        public void StickToCollidedObject(Collision collision)
        {
            var rb = GetComponentInChildren<Rigidbody>();
            
            if (rb != null)
            {
                rb.detectCollisions = false;
                rb.isKinematic = true;
                transform.SetParent(collision.transform.localScale != Vector3.one ? collision.transform.parent : collision.transform, true);
            }
        }
    }
}