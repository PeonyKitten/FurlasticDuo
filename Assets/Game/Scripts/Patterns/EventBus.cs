using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Scripts.Patterns
{
    public static class EventBus<T>
    {
        private static readonly IDictionary<T, UnityEvent> Events = new Dictionary<T, UnityEvent>();

        public static void Subscribe(T eventType, UnityAction listener)
        {
            if (Events.TryGetValue(eventType, out var thisEvent))
            {
                thisEvent.AddListener(listener);
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                Events.Add(eventType, thisEvent);
            }
        }

        public static void Unsubscribe(T eventType, UnityAction listener)
        {
            if (Events.TryGetValue(eventType, out var thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void Publish(T eventType)
        {
            if (Events.TryGetValue(eventType, out var thisEvent))
            {
                thisEvent.Invoke();
            }
        }
    }
}