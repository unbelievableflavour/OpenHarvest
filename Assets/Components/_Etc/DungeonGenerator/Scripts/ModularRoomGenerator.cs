using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ModularRoomGenerator : ModularGenerator
{
    public int Iterations = 3;

    public Module[] Modules;

    [Header("Filled on start")]
    [ReadOnly]
    public List<Module> allSpawnedModulesExceptStart;

    public void SpawnModulesInRoom(Module spawnedStartModule)
    {
        var currentIterationOfModuleConnectors = new List<PropModuleConnector>(spawnedStartModule.GetPropExits());

        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            var nextIterationModuleConnectors = new List<PropModuleConnector>();

            foreach (var currentModule in currentIterationOfModuleConnectors)
            {
                if (Random.Range(1, 100) > currentModule.spawnRate)
                {
                    continue;
                }

                string newTag = GetRandomTagToSpawn(currentModule);
                var newModulePrefab = GetRandomModuleWithTag(Modules, newTag);
                var newModule = (Module)Instantiate(newModulePrefab);
                var newModuleColliders = newModule.Colliders;

                var newModuleExits = newModule.GetPropExits();
                var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetModuleConnectorWithTag(newModuleExits, newTag);
                ConnectModules(currentModule, exitToMatch);

                allSpawnedModulesExceptStart.Add(newModule);
                nextIterationModuleConnectors.AddRange(newModuleExits.Where(e => e != exitToMatch));
            }

            currentIterationOfModuleConnectors = nextIterationModuleConnectors;
        }

        //Disable all modules
        foreach (Module module in allSpawnedModulesExceptStart)
        {
            module.gameObject.SetActive(false);
        }
    }
}
