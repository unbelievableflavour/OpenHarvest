using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class View
{
    public string id;
    public GameObject view;
}

public class ViewSwitcher : MonoBehaviour
{
    public List<View> views = new List<View>();
    public View currentView;

    public event Action<string> OnViewChanged;

    void Awake()
    {
        EnsureDefaultCurrentView();
    }

    void Start()
    {
        EnsureDefaultCurrentView();
        if (views == null)
        {
            return;
        }

        for (int i = 0; i < views.Count; i++)
        {
            View view = views[i];
            if (view.view == null)
            {
                continue;
            }

            view.view.SetActive(view.view == currentView.view);
        }
    }

    private void EnsureDefaultCurrentView()
    {
        if (views == null || views.Count == 0)
        {
            return;
        }

        if (currentView.view == null)
        {
            currentView = views[0];
        }
    }

    public void setActiveView(string id)
    {
        EnsureDefaultCurrentView();
        if (currentView.view != null)
        {
            currentView.view.SetActive(false);
        }

        foreach (View view in views)
        {
            if(view.id != id)
            {
                continue;
            }
            currentView = view;
            view.view.SetActive(true);
            OnViewChanged?.Invoke(id);
        }
    }
}
