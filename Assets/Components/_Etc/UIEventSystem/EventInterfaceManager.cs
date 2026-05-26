using System;
using System.Collections.Generic;
using UnityEngine;

public class EventInterfaceManager : MonoBehaviour
{
    [SerializeField] private ViewSwitcher viewSwitcher;

    private class Subscription
    {
        public string eventName;
        public Action handler;
    }

    private readonly List<Subscription> activeSubscriptions = new List<Subscription>();

    private void OnEnable()
    {
        activeSubscriptions.Clear();

        if (viewSwitcher == null)
        {
            return;
        }

        if (viewSwitcher.views == null)
        {
            return;
        }

        var seen = new HashSet<string>();
        for (int i = 0; i < viewSwitcher.views.Count; i++)
        {
            var view = viewSwitcher.views[i];
            if (view == null)
            {
                continue;
            }

            var name = view.id;
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (!seen.Add(name))
            {
                continue;
            }

            var captured = name;
            Action handler = () => viewSwitcher.setActiveView(captured);

            var subscription = new Subscription
            {
                eventName = captured,
                handler = handler
            };

            activeSubscriptions.Add(subscription);
            EventManager.Subscribe(subscription.eventName, subscription.handler);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < activeSubscriptions.Count; i++)
        {
            var sub = activeSubscriptions[i];
            if (sub == null)
            {
                continue;
            }

            EventManager.Unsubscribe(sub.eventName, sub.handler);
        }

        activeSubscriptions.Clear();
    }
}
