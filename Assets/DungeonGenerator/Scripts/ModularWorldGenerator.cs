using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ModularWorldGenerator : ModularGenerator
{
    public HarvestSettings harvestSettings;
    public ModularRoomGenerator modularRoomGenerator;
    public int Iterations = 5;

    [Header("Module types")]
    public Module StartModule;
    public Module BlockModule;
    public GameObject Threshold;
    public Module[] Modules;

    [Header("Debug Tools")]
    public Material previewMaterial;

    [Header("Filled on start")]
    [ReadOnly]
    public List<Module> allSpawnedModulesExceptStart;

    //Use for player location determination
    public static Module currentModule;
    int maxShowDistance = 30;

    Module spawnedStartModule;

    void Start() 
    {
        Reset();
        spawnedStartModule = (Module)Instantiate(StartModule, transform.position, transform.rotation);
        foreach (var newModuleCollider in spawnedStartModule.Colliders)
        {
            newModuleCollider.gameObject.layer = 18;
        }

        var currentIterationModuleConnectors = new List<MapModuleConnector>(spawnedStartModule.GetMapExits());

        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            var nextIterationModuleConnectors = new List<MapModuleConnector>();

            foreach (var currentModule in currentIterationModuleConnectors)
            {
                string newTag = GetRandomTagToSpawn(currentModule);
                var newModulePrefab = GetRandomModuleWithTag(Modules, newTag);
                var newModule = (Module)Instantiate(newModulePrefab);
                var newModuleColliders = newModule.Colliders;

                var newModuleExits = newModule.GetMapExits();
                var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetModuleConnectorWithTag(newModuleExits, newTag);
                ConnectModules(currentModule, exitToMatch);

                if (moduleOverlapsOtherModule(newModuleColliders)) {
                    Destroy(newModule.gameObject);
                    SpawnBlockade(currentModule);
                    continue;
                }

                foreach (var newModuleCollider in newModuleColliders)
                {
                    // prepare the unprepared module collider
                    newModuleCollider.gameObject.layer = 18;
                }

                SpawnThreshold(currentModule);

                modularRoomGenerator.SpawnModulesInRoom(newModule);

                allSpawnedModulesExceptStart.Add(newModule);
                nextIterationModuleConnectors.AddRange(newModuleExits.Where(e => e != exitToMatch));
            }

            currentIterationModuleConnectors = nextIterationModuleConnectors;
        }

        lockExitsOfFinalIteration(currentIterationModuleConnectors);

        //Disable all modules
        foreach (Module module in allSpawnedModulesExceptStart)
        {
            
#if UNITY_EDITOR
            if (harvestSettings.showAllModulesOnStart)
            {
                break;
            }
#endif

            module.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(currentModule == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (harvestSettings.isPCMode)
        {
            return;
        }
#endif

        // If player somehow manages to fall of the map, go to farm entrance.
        var moduleCenter = currentModule.Colliders[0].bounds.center;
        float currentDistance = Vector3.Distance(moduleCenter, GameState.Instance.currentPlayerPosition.position);
        bool tooFarAway = currentDistance > maxShowDistance;

        if (!tooFarAway)
        {
            return;
        }
              
        SceneSwitcher.Instance.SwitchToScene(1, "Cave");
    }

    private bool moduleOverlapsOtherModule(Collider[] newModuleColliders) {
        foreach (Collider newModuleCollider in newModuleColliders) {
            if (colliderOverlapsOtherModule(newModuleCollider)) {
                return true;
            }
        }

        return false;
    }

    private void lockExitsOfFinalIteration(List<MapModuleConnector> moduleConnectors){
        foreach (MapModuleConnector moduleConnector in moduleConnectors) {
            SpawnBlockade(moduleConnector);
        }
    }

    private void SpawnBlockade(ModuleConnector moduleConnector)
    {
        var blockModule = (Module)Instantiate(BlockModule);
        var newModuleExits = blockModule.GetExits();
        var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetRandom(newModuleExits);
        ConnectModules(moduleConnector, exitToMatch);
        allSpawnedModulesExceptStart.Add(blockModule);
    }

    //This is a WIP fix for the players falling through the map it places a small collider between every doorway so people dont fall.
    private void SpawnThreshold(ModuleConnector moduleConnector)
    {
        var threshold = (GameObject)Instantiate(Threshold);
        threshold.transform.rotation = moduleConnector.transform.rotation;
        threshold.transform.position = moduleConnector.transform.position;
        threshold.transform.parent = moduleConnector.transform;
    }

    private void Reset() {
        currentModule = null;
        Module.lastModule = null;
    }

    bool colliderOverlapsOtherModule(Collider newModuleCollider)
    {
        if (newModuleCollider.GetComponent<BoxCollider>())
        {
            var boxCollider = newModuleCollider.GetComponent<BoxCollider>();

            Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);

#if UNITY_EDITOR
            if (harvestSettings.showOverlapColliders)
            {
                generateDebugCube(boxCollider, false, "OVERLAP_COLLIDER");
            }
#endif

            Collider[] hitColliders = Physics.OverlapBox(worldCenter, boxCollider.size * 0.5f, boxCollider.transform.rotation);
            foreach (var hitCollider in hitColliders)
            {
                //If collision is of preparedModuleColliders
                if (hitCollider.gameObject.layer == 18)
                {
#if UNITY_EDITOR
                    if (harvestSettings.showRemovedOverlapColliders)
                    {
                        generateDebugCube(
                            boxCollider,
                            true,
                            "REMOVED_OVERLAP_COLLIDER - " + newModuleCollider.name + " against " + hitCollider.name
                        );
                    }
#endif
                    return true;
                }
            }
        }

        if (newModuleCollider.GetComponent<SphereCollider>())
        {
            SphereCollider sphereCollider = newModuleCollider.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(sphereCollider.bounds.center, sphereCollider.radius);

#if UNITY_EDITOR
            if (harvestSettings.showOverlapColliders)
            {
                generateDebugSphere(sphereCollider, false, "OVERLAP_COLLIDER");
            }
#endif

            foreach (var hitCollider in hitColliders)
            {

                if (hitCollider.gameObject.layer == 18)
                {
#if UNITY_EDITOR
                    if (harvestSettings.showRemovedOverlapColliders)
                    {
                        generateDebugSphere(
                            sphereCollider,
                            true,
                            "REMOVED_OVERLAP_COLLIDER - " + newModuleCollider.name + " against " + hitCollider.name
                        );
                    }
#endif

                    return true;
                }
            }
        }

        return false;
    }

    private void generateDebugCube(BoxCollider boxCollider, bool isRemoved = false, string name = null)
    {
        Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.name = string.IsNullOrEmpty(name) ? "DEBUG - BoxCollider" : name;
        cube.transform.position = worldCenter;
        cube.transform.localScale = boxCollider.size;
        cube.transform.rotation = boxCollider.transform.rotation;

        if (isRemoved)
        {
            var renderer = cube.GetComponent<Renderer>();
            renderer.material = previewMaterial;
        }
    }

    private void generateDebugSphere(SphereCollider sphereCollider, bool isRemoved = false, string name = null)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.name = string.IsNullOrEmpty(name) ? "DEBUG - SphereCollider" : name;
        sphere.transform.position = sphereCollider.bounds.center;
        sphere.transform.localScale = new Vector3(sphereCollider.radius, sphereCollider.radius, sphereCollider.radius) * 2;

        if (isRemoved)
        {
            var renderer = sphere.GetComponent<Renderer>();
            renderer.material = previewMaterial;
        }
    }
}
