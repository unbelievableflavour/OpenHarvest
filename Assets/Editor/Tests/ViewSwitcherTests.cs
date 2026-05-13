using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class ViewSwitcherTests
{
    [Test]
    public void SetActiveView_DoesNotThrow_WhenCurrentViewHasNullView()
    {
        var root = new GameObject("ViewSwitcherRoot");
        var listView = new GameObject("ListView");
        var detailsView = new GameObject("DetailsView");
        listView.transform.SetParent(root.transform, false);
        detailsView.transform.SetParent(root.transform, false);

        try
        {
            ViewSwitcher switcher = root.AddComponent<ViewSwitcher>();
            switcher.views = new List<View>
            {
                new View { id = "list", view = listView },
                new View { id = "details", view = detailsView },
            };
            switcher.currentView = new View { id = string.Empty, view = null };

            Assert.DoesNotThrow(() => switcher.setActiveView("list"));
            Assert.IsTrue(listView.activeSelf);
            Assert.IsFalse(detailsView.activeSelf);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }
}
