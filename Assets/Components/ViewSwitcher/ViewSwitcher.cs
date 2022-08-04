using System.Collections.Generic;
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

    void Start()
    {
        currentView = views[0];
    }

    public void setActiveView(string id)
    {
        currentView.view.SetActive(false);

        foreach (View view in views)
        {
            if(view.id != id)
            {
                continue;
            }
            currentView = view;
            view.view.SetActive(true);
        }
    }
}
