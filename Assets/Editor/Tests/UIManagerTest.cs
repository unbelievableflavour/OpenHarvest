using System.Collections.Generic;
using BNG;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class UIManagerTest
{
    GameObject root;
    ViewSwitcher viewSwitcher;
    FirstPersonController fpsController;
    SmoothLocomotion smoothLocomotion;
    PlayerTeleport playerTeleport;

    [SetUp]
    public void SetUp()
    {
        root = new GameObject("Root");
        viewSwitcher = CreateViewSwitcher();
        fpsController = CreateFpsController();
        smoothLocomotion = Child("SmoothLocomotion").AddComponent<SmoothLocomotion>();
        playerTeleport = Child("PlayerTeleport").AddComponent<PlayerTeleport>();
        CreateUIManager();
    }

    [Test]
    public void WhenDefaultViewRestored_PCMovementIsRestored()
    {
        viewSwitcher.setActiveView("shop");
        viewSwitcher.setActiveView("default");

        Assert.IsTrue(fpsController.playerCanMove);
        Assert.IsTrue(fpsController.cameraCanMove);
    }

    [Test]
    public void WhenDefaultViewRestored_VRLocomotionIsRestored()
    {
        viewSwitcher.setActiveView("shop");
        viewSwitcher.setActiveView("default");

        Assert.IsTrue(smoothLocomotion.AllowInput);
    }

    [Test]
    public void WhenDefaultViewRestored_VRTeleportIsEnabled()
    {
        viewSwitcher.setActiveView("shop");
        viewSwitcher.setActiveView("default");

        Assert.IsTrue(playerTeleport.enabled);
    }

    [Test]
    public void WhenViewOpens_PreexistingDisabledStateIsPreserved()
    {
        smoothLocomotion.AllowInput = false;

        viewSwitcher.setActiveView("shop");
        viewSwitcher.setActiveView("default");

        Assert.IsFalse(smoothLocomotion.AllowInput);
    }

    [TearDown]
    public void Cleanup()
    {
        Object.DestroyImmediate(root);
    }

    private ViewSwitcher CreateViewSwitcher()
    {
        var vs = Child("ViewSwitcher").AddComponent<ViewSwitcher>();
        vs.views = new List<View>
        {
            new View { id = "default", view = Child("DefaultView") },
            new View { id = "shop",    view = Child("ShopView") },
        };
        vs.currentView = vs.views[0];
        return vs;
    }

    private FirstPersonController CreateFpsController()
    {
        var go = Child("FPSController");
        go.SetActive(false); // defer Awake until required fields are assigned
        go.AddComponent<Rigidbody>();
        var fps = go.AddComponent<FirstPersonController>();
        fps.playerCamera = Child("Camera", go).AddComponent<Camera>();
        fps.joint = Child("Joint", go).transform;
        go.SetActive(true);
        return fps;
    }

    private void CreateUIManager()
    {
        var go = Child("UIManager");
        go.SetActive(false); // defer Awake/OnEnable until dependencies are injected
        var ui = go.AddComponent<UIManager>();
        var so = new SerializedObject(ui);
        so.FindProperty("viewSwitcher").objectReferenceValue = viewSwitcher;
        so.FindProperty("fpsController").objectReferenceValue = fpsController;
        so.FindProperty("smoothLocomotion").objectReferenceValue = smoothLocomotion;
        so.FindProperty("playerTeleport").objectReferenceValue = playerTeleport;
        so.ApplyModifiedProperties();
        go.SetActive(true);
    }

    private GameObject Child(string name, GameObject parent = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent((parent ?? root).transform, false);
        return go;
    }
}
