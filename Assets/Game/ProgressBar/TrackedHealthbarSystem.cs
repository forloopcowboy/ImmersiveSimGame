using System;
using System.Collections.Generic;
using Game.HealthSystem;
using Game.Src.EventBusModule;
using Game.Utils;
using UnityEngine;

namespace Game.ProgressBar
{
    public class TrackedHealthbarSystem : SingletonMonoBehaviour<TrackedHealthbarSystem>
    {
        public HealthBar healthBarPrefab;
        
        private Dictionary<Health, HealthBar> _trackedHealthbars = new();
        private Action unsubTracking;
        private Action unsubTrackingStop;

        private void OnEnable()
        {
            unsubTracking = SceneEventBus.Subscribe<TrackHealthEvent>(OnHealthTrackEvent);
            unsubTrackingStop = SceneEventBus.Subscribe<StopTrackingHealthEvent>(OnHealthStopTrackEvent);
        }

        private void OnDisable()
        {
            unsubTracking?.Invoke();
            unsubTrackingStop?.Invoke();
        }

        private void OnHealthTrackEvent(TrackHealthEvent obj)
        {
            if (!_trackedHealthbars.ContainsKey(obj.health))
                _trackedHealthbars.Add(obj.health, Instantiate(healthBarPrefab, transform));

            if (_trackedHealthbars.TryGetValue(obj.health, out HealthBar healthBar))
            {
                healthBar.health = obj.health;
                var positionFollower = healthBar.GetOrElseAddComponent<CanvasFollowWorldPosition>();
                positionFollower.lookAt = obj.health.transform;
                positionFollower.canvas = GetComponentInChildren<Canvas>() ?? GetComponentInParent<Canvas>();
            }
        }
        
        private void OnHealthStopTrackEvent(StopTrackingHealthEvent obj)
        {
            if (_trackedHealthbars.ContainsKey(obj.health))
            {
                Destroy(_trackedHealthbars[obj.health].gameObject);
                _trackedHealthbars.Remove(obj.health);
            }
        }
    }

    public class TrackHealthEvent
    {
        public Health health;

        public TrackHealthEvent(Health health)
        {
            this.health = health;
        }
    }

    public class StopTrackingHealthEvent
    {
        public Health health;

        public StopTrackingHealthEvent(Health health)
        {
            this.health = health;
        }
    }
}