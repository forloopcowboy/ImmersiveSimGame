using System;
using System.Collections.Generic;
using Game.Utils;
using Game.Utils.EventBusModule;
using UnityEngine;

namespace Game.Src.EventBusModule
{
    public class SceneEventBus : SingletonMonoBehaviour<SceneEventBus>
    {
        private EventBus bus = new();

        public bool debug = false;
        private bool ShowDebug() => debug;

        private void Start()
        {
            Debug.Log("Starting SceneEventBus. Total number of listeners: " + totalNumberOfListeners);
            DontDestroyOnLoad(gameObject);
        }

        // [ShowInInspector, FoldoutGroup("Debug"), ShowIf("ShowDebug")]
        private int totalNumberOfListeners => bus.GetSubscriberCount();
        // [ShowInInspector, FoldoutGroup("Debug"), ShowIf("ShowDebug")]
        private List<Type> activeEventTypes => bus.GetActiveEvents();

        /** Emits an event to all handlers of TEvent. */
        public static void Emit<TEvent>(TEvent @event) => Singleton?.bus.Emit(@event);
        
        /** Subscribes to all events TEvent. @returns Action to unsubscribe. */
        public static Action Subscribe<TEvent>(Action<TEvent> handler) => Singleton?.bus.Subscribe(handler);
    }
}