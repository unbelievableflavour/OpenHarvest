using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ModularGenerator : MonoBehaviour
{
    protected void ConnectModules(ModuleConnector oldExit, ModuleConnector newExit)
    {
        var newModule = newExit.transform.parent;
        var forwardVectorToMatch = -oldExit.transform.forward;
        var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
        newModule.RotateAround(newExit.transform.position, Vector3.up, correctiveRotation);
        var correctiveTranslation = oldExit.transform.position - newExit.transform.position;
        newModule.transform.position += correctiveTranslation;

        oldExit.connectedModuleConnector = newExit;
        newExit.connectedModuleConnector = oldExit;
    }

    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
    }

    protected TItem GetRandom<TItem>(TItem[] array)
    {
        int randomIndex = Random.Range(0, array.Length);
        return array[randomIndex];
    }

    protected string GetRandomTagToSpawn(ModuleConnector moduleConnector)
    {
        try {
            return GetRandom(moduleConnector.CanSpawn);
        } catch {
            throw new System.Exception("index error occured in a ModuleConnector of module: " + moduleConnector.GetModule().transform.name);
        }
        return GetRandom(moduleConnector.CanSpawn);
    }
  
    protected Module GetRandomModuleWithTag(IEnumerable<Module> modules, string tagToMatch)
    {
        var matchingModules = modules.Where(m => m.Tags.Contains(tagToMatch)).ToArray();
        if (matchingModules.Length == 0)
        {
            throw new System.Exception("No modules found with tag: " + tagToMatch);
        }
        return GetRandom(matchingModules);
    }

    protected ModuleConnector GetModuleConnectorWithTag(ModuleConnector[] moduleConnectors, string tagToMatch)
    {
        var matchingModuleConnectors = moduleConnectors.Where(m => m.CanReceive.Contains(tagToMatch)).ToArray();
        if (matchingModuleConnectors.Length == 0)
        {
            throw new System.Exception("No module connectors found with tag: " + tagToMatch);
        }
        return GetRandom(matchingModuleConnectors);
    }
}
