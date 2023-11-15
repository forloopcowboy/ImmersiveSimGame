using System;
using System.Collections;
using Game.HealthSystem;
using Game.Src.EventBusModule;
using Game.Utils;
using UnityEngine;

namespace Game.ProgressBar
{
    public class TrackHealth : MonoBehaviour
    {
        private Health _health;
        private bool _notifiedDeath = false;
        
        private void OnEnable()
        {
            _health = GetComponentInChildren<Health>();
            StartCoroutine(CoroutineHelpers.DelayedAction(0.25f, () =>
            {
                SceneEventBus.Emit(new TrackHealthEvent(_health));
            }));
        }

        private void Update()
        {
            if (_health.isDead && !_notifiedDeath)
            {
                _notifiedDeath = true;
                SceneEventBus.Emit(new StopTrackingHealthEvent(_health));
            }
        }

        private void OnDisable()
        {
            SceneEventBus.Emit(new StopTrackingHealthEvent(_health));
        }
    }
}