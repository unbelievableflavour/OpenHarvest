using System.Collections.Generic;
using System.Reflection;
using BNG;
using NUnit.Framework;
using Tests;
using UnityEditor;
using UnityEngine;

public class UIManagerTest
{
    [Test]
    public void ClosingAllUiAfterSwitchingToTeleport_ReenablesPlayerTeleport()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Components/_Etc/Player/NewCustomPlayerAdvanced Variant.prefab");
        var player = Object.Instantiate(prefab);
        Transform xrRigAdvanced = player.transform.Find("XR Rig Advanced");

        var uiRoot = new GameObject("UIManagerTestRoot");
        var viewSwitcher = uiRoot.AddComponent<ViewSwitcher>();
        var uiManager = uiRoot.AddComponent<UIManager>();
        var mainView = new GameObject("MainView");
        var settingsView = new GameObject("SettingsView");
        mainView.transform.SetParent(uiRoot.transform, false);
        settingsView.transform.SetParent(uiRoot.transform, false);

        try
        {
            var locomotionManager = xrRigAdvanced.GetComponentInChildren<LocomotionManager>();
            var playerTeleport = xrRigAdvanced.GetComponentInChildren<PlayerTeleport>();

            locomotionManager.ChangeLocomotion(LocomotionType.SmoothLocomotion, false);
            Assert.IsFalse(playerTeleport.enabled);

            viewSwitcher.views = new List<View>
            {
                new View { id = "default", view = mainView },
                new View { id = "settings", view = settingsView },
            };
            viewSwitcher.currentView = viewSwitcher.views[0];

            SetPrivateField(uiManager, "viewSwitcher", viewSwitcher);
            SetPrivateField(uiManager, "mainViewId", "default");

            EditModeLifecycle.InvokeAwake(uiManager);
            ClearAdditionalViewSwitchers(uiManager);
            SetPrivateField(uiManager, "locomotionManager", locomotionManager);
            SetPrivateField(uiManager, "playerTeleport", playerTeleport);
            EditModeLifecycle.InvokeOnEnable(uiManager);

            viewSwitcher.setActiveView("settings");
            Assert.IsFalse(playerTeleport.enabled);

            locomotionManager.ChangeLocomotion(LocomotionType.Teleport, false);
            uiManager.HandleLocomotionSettingsChanged();
            Assert.IsFalse(playerTeleport.enabled);

            viewSwitcher.setActiveView("default");
            Assert.IsTrue(playerTeleport.enabled);
        }
        finally
        {
            Object.DestroyImmediate(uiRoot);
            Object.DestroyImmediate(player);
        }
    }

    [Test]
    public void ClosingSettingsWhileMenuStillOpen_KeepsPlayerTeleportDisabled()
    {
        var uiRoot = new GameObject("UIManagerTestRoot");
        var viewSwitcher = uiRoot.AddComponent<ViewSwitcher>();
        var uiManager = uiRoot.AddComponent<UIManager>();
        var mainView = new GameObject("MainView");
        var menuView = new GameObject("MenuView");
        var settingsView = new GameObject("SettingsView");
        mainView.transform.SetParent(uiRoot.transform, false);
        menuView.transform.SetParent(uiRoot.transform, false);
        settingsView.transform.SetParent(uiRoot.transform, false);

        var playerRoot = new GameObject("PlayerRoot");
        LocomotionManager locomotionManager = playerRoot.AddComponent<LocomotionManager>();
        var teleportObject = new GameObject("PlayerTeleport");
        teleportObject.transform.SetParent(playerRoot.transform, false);
        PlayerTeleport playerTeleport = teleportObject.AddComponent<PlayerTeleport>();

        try
        {
            locomotionManager.ChangeLocomotion(LocomotionType.Teleport, false);
            Assert.IsTrue(playerTeleport.enabled);

            viewSwitcher.views = new List<View>
            {
                new View { id = "default", view = mainView },
                new View { id = "menu", view = menuView },
                new View { id = "settings", view = settingsView },
            };
            viewSwitcher.currentView = viewSwitcher.views[1];

            SetPrivateField(uiManager, "viewSwitcher", viewSwitcher);
            SetPrivateField(uiManager, "mainViewId", "default");

            EditModeLifecycle.InvokeAwake(uiManager);
            ClearAdditionalViewSwitchers(uiManager);
            SetPrivateField(uiManager, "locomotionManager", locomotionManager);
            SetPrivateField(uiManager, "playerTeleport", playerTeleport);
            EditModeLifecycle.InvokeOnEnable(uiManager);
            Assert.IsFalse(playerTeleport.enabled);

            viewSwitcher.setActiveView("settings");
            Assert.IsFalse(playerTeleport.enabled);

            viewSwitcher.setActiveView("menu");
            Assert.IsFalse(playerTeleport.enabled);

            viewSwitcher.setActiveView("default");
            Assert.IsTrue(playerTeleport.enabled);
        }
        finally
        {
            Object.DestroyImmediate(playerRoot);
            Object.DestroyImmediate(uiRoot);
        }
    }

    static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.SetValue(target, value);
    }

    static void ClearAdditionalViewSwitchers(UIManager uiManager)
    {
        FieldInfo field = typeof(UIManager).GetField(
            "additionalViewSwitchers",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var additionalViewSwitchers = (List<ViewSwitcher>)field.GetValue(uiManager);
        additionalViewSwitchers.Clear();
    }

}
