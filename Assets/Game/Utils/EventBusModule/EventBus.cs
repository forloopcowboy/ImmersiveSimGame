using System;
using System.Collections.Generic;

namespace Game.Utils.EventBusModule
{
    public interface IEventBus
    {
        void Emit<TEvent>(TEvent @event);
        Action Subscribe<TEvent>(Action<TEvent> handler);
    }
    
    public class EventBus : IEventBus
    {
        private Dictionary<Type, List<Action<dynamic>>> handlers = new();

        public virtual void Emit<TEvent>(TEvent @event)
        {
            if (handlers == null) handlers = new Dictionary<Type, List<Action<dynamic>>>();
            
            if (handlers.TryGetValue(typeof(TEvent), out var theseHandlers))
            {
                foreach (var handler in theseHandlers)
                {
                    handler(@event);
                }
            }
        }

        /// <summary>
        /// Returns function to unsubscribe.
        /// </summary>
        /// <param name="handler"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public virtual Action Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handlers == null) handlers = new Dictionary<Type, List<Action<dynamic>>>();
            
            void WrappedHandler(dynamic dynamicArg)
            {
                if (dynamicArg is TEvent @event)
                    handler(@event);
            }
            
            if (handlers.TryGetValue(typeof(TEvent), out var handlerList))
                handlerList.Add(WrappedHandler);
            else
            {
                var newHandlerList = new List<Action<dynamic>>();
                newHandlerList.Add(WrappedHandler);
                
                handlers.Add(
                    typeof(TEvent),
                    newHandlerList
                );
            }
            
            return () =>
            {
                handlers[typeof(TEvent)].Remove(WrappedHandler);
            };
        }
        
        public static Action<T> Debounce<T>(float seconds, Action<T> action)
        {
            var lastExecutionTime = DateTime.MinValue;

            Action<T> debouncedAction = arg =>
            {
                var now = DateTime.Now;
                var timeSinceLastExecution = now - lastExecutionTime;

                if (timeSinceLastExecution.TotalSeconds >= seconds)
                {
                    action(arg);
                    lastExecutionTime = now;
                }
            };

            return debouncedAction;
        }
        
        /// <returns>Total number of active subscribers.</returns>
        public int GetSubscriberCount()
        {
            if (handlers == null) handlers = new Dictionary<Type, List<Action<dynamic>>>();
            
            int total = 0;
            foreach (var keyValuePair in handlers)
                total += keyValuePair.Value.Count;

            return total;
        }
        
        /// <returns>Number of event types that have >1 active subscriber.</returns>
        public List<Type> GetActiveEvents()
        {
            if (handlers == null) handlers = new Dictionary<Type, List<Action<dynamic>>>();
            
            List<Type> total = new();
            foreach (var keyValuePair in handlers)
            {
                if (keyValuePair.Value.Count > 0)
                    total.Add(keyValuePair.Key);
            }
            
            return total;
        }
    }
}