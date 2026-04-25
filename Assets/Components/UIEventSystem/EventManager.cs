using System;
using System.Collections.Generic;

public static class EventManager
{
    private static readonly Dictionary<string, List<Action>> listenersByName =
        new Dictionary<string, List<Action>>();

    public static void Subscribe(string eventName, Action callback)
    {
        if (string.IsNullOrWhiteSpace(eventName) || callback == null)
        {
            return;
        }

        if (!listenersByName.TryGetValue(eventName, out var list))
        {
            list = new List<Action>();
            listenersByName[eventName] = list;
        }

        if (!list.Contains(callback))
        {
            list.Add(callback);
        }
    }

    public static void Unsubscribe(string eventName, Action callback)
    {
        if (string.IsNullOrWhiteSpace(eventName) || callback == null)
        {
            return;
        }

        if (!listenersByName.TryGetValue(eventName, out var list))
        {
            return;
        }

        list.Remove(callback);
        if (list.Count == 0)
        {
            listenersByName.Remove(eventName);
        }
    }

    public static void Emit(string eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            return;
        }

        if (!listenersByName.TryGetValue(eventName, out var list) || list.Count == 0)
        {
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i]?.Invoke();
        }
    }
}
