using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class Module : MonoBehaviour
{
	public Collider[] Colliders;
    public static Module lastModule = null;

	public ModuleConnector[] GetExits()
	{
		return GetComponentsInChildren<ModuleConnector>();
	}
    
    public bool ModuleConnectorHasTag(string tagToMatch)
	{
        var moduleConnectors = GetExits();
		var matchingModuleConnectors = moduleConnectors.Where(moduleConnector => moduleConnector.CanReceive.Contains(tagToMatch)).ToArray();
        return matchingModuleConnectors.Length > 0;
	}

    public PropModuleConnector[] GetPropExits()
	{
		return GetComponentsInChildren<PropModuleConnector>();
	}

    public MapModuleConnector[] GetMapExits()
	{
		return GetComponentsInChildren<MapModuleConnector>();
	}

    private void ToggleConnectedModules(bool isOn)
    {
        foreach (var exit in GetExits())
        {
            ToggleConnectedModule(exit, isOn);
        }
    }

    private void ToggleConnectedModulesOfConnectedModules(bool isOn)
    {
        foreach (var exit in GetExits())
        {
            ToggleConnectedModuleOfConnectedModule(exit, isOn);
        }
    }

    public void ToggleModule()
    {
        if (lastModule)
        {
            this.DisableConnectedModulesOfPreviousCurrent(false);
        }

        lastModule = this;

        this.ToggleConnectedModules(true);
        this.ToggleConnectedModulesOfConnectedModules(true);
    }

    public void DisableConnectedModulesOfPreviousCurrent(bool isOn)
    {
        foreach (var exit in lastModule.GetExits())
        {
            ToggleConnectedModuleOfConnectedModule(exit, isOn);
        }
    }

    private void ToggleConnectedModule(ModuleConnector exit, bool isOn)
    {
        if (!exit.connectedModuleConnector || !exit.connectedModuleConnector.GetModule())
        {
            return;
        }

        exit.connectedModuleConnector.GetModule().gameObject.SetActive(isOn);
    }

    private void ToggleConnectedModuleOfConnectedModule(ModuleConnector exit, bool isOn)
    {
        if (!exit.connectedModuleConnector || !exit.connectedModuleConnector.GetModule())
        {
            return;
        }

        foreach (var depth2exit in exit.connectedModuleConnector.GetModule().GetExits())
        {
            ToggleConnectedModule(depth2exit, isOn);
        }      
    }
}
