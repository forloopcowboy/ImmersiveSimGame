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
            _health.onDamage.AddListener(OnDamageTrackHealth);
        }

        private void OnDamageTrackHealth(GameObject arg0)
        {
            SceneEventBus.Emit(new TrackHealthEvent(_health));
            _health.onDamage.RemoveListener(OnDamageTrackHealth);
        }

        private void Update()
        {
            if (_health.isDead && !_notifiedDeath)
            {
                _notifiedDeath = true;
                StartCoroutine(CoroutineHelpers.DelayedAction(1.25f, () =>
                {
                    SceneEventBus.Emit(new StopTrackingHealthEvent(_health));
                }));
            }
        }

        private void OnDisable()
        {
            SceneEventBus.Emit(new StopTrackingHealthEvent(_health));
        }
    }
}