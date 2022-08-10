using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class ModularWorldGenerator : MonoBehaviour
{
    public Module StartModule;
    public Module BlockModule;
    public GameObject Threshold;
    public Module[] Modules;
    public int Iterations = 5;

    [Header("Debug Tools")]
    public bool showAllModulesOnStart = false;
    public bool showOverlapColliders = false;
    public bool showRemovedOverlapColliders = false;
    public Material previewMaterial;

    [Header("Filled on start")]

    [ReadOnly]
    public List<Module> allModulesExceptStart;

    //Use for player location determination
    public static Module currentModule;
    int maxShowDistance = 30;

    Module startModule;

    void Start()
    {
        Reset();
        startModule = (Module)Instantiate(StartModule, transform.position, transform.rotation);
        foreach (var newModuleCollider in startModule.Colliders)
        {
            newModuleCollider.gameObject.layer = 18;
        }

        var pendingExits = new List<ModuleConnector>(startModule.GetExits());
        //var pendingTag = startModule.Tags;
        for (int iteration = 0; iteration < Iterations; iteration++)
        {
            var newExits = new List<ModuleConnector>();

            foreach (var pendingExit in pendingExits)
            {
                if (Random.Range(1, 100) > pendingExit.spawnRate)
                {
                    continue;
                }
                string newTag = "";

                try
                {
                    newTag = GetRandom(pendingExit.CanSpawn);
                }
                catch
                {
                    throw new System.Exception("index error occured for module: " + pendingExit.GetModule().transform.name);
                }

                var newModulePrefab = GetRandomWithTag(Modules, newTag);
                var newModule = (Module)Instantiate(newModulePrefab);
                var newModuleColliders = newModule.Colliders;

                //Collider collision = newModule.GetComponent<Module>().Colliders[0];

                var newModuleExits = newModule.GetExits();
                var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetRandomExitWithTag(newModuleExits, newTag);
                ConnectExits(pendingExit, exitToMatch);

                bool shouldKeep = true;
                foreach (var newModuleCollider in newModuleColliders)
                {
                    if (colliderOverlapsOtherModule(newModuleCollider))
                    {
                        shouldKeep = false;
                        break;
                    }

                    //This collider checks out!
                    // prepare the unprepared module collider
                    newModuleCollider.gameObject.layer = 18;
                }

                if (!shouldKeep)
                {
                    Destroy(newModule.gameObject);
                    SpawnBlockade(pendingExit);
                    continue;
                }

                if (pendingExit.needsBlockade)
                {
                    SpawnThreshold(pendingExit);
                }

                pendingExit.connectedModuleConnector = exitToMatch;
                exitToMatch.connectedModuleConnector = pendingExit;

                allModulesExceptStart.Add(newModule);
                newExits.AddRange(newModuleExits.Where(e => e != exitToMatch));
            }

            pendingExits = newExits;
        }

        //lockUpOpenExits
        foreach (var pendingExit in pendingExits)
        {
            if (pendingExit.needsBlockade)
            {
                SpawnBlockade(pendingExit);
            }
        }

        //Disable all modules except the first + connected.
        foreach (Module module in allModulesExceptStart)
        {
            if (showAllModulesOnStart)
            {
                return;
            }
            module.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(currentModule == null)
        {
            return;
        }

        var moduleCenter = currentModule.Colliders[0].bounds.center;
        float currentDistance = Vector3.Distance(moduleCenter, GameState.currentPlayerPosition.position);
        bool tooFarAway = currentDistance > maxShowDistance;

        if (!tooFarAway)
        {
            return;
        }

        // //Disable all modules except the first + connected.
        // foreach (Module module in allModulesExceptStart)
        // {
        //     module.gameObject.SetActive(false);
        // }

        // startModule.ToggleModule();
        GameState.currentSceneSwitcher.SwitchToScene(1, "Cave");
    }

    private void ConnectExits(ModuleConnector oldExit, ModuleConnector newExit)
    {
        var newModule = newExit.transform.parent;
        var forwardVectorToMatch = -oldExit.transform.forward;
        var correctiveRotation = Azimuth(forwardVectorToMatch) - Azimuth(newExit.transform.forward);
        newModule.RotateAround(newExit.transform.position, Vector3.up, correctiveRotation);
        var correctiveTranslation = oldExit.transform.position - newExit.transform.position;
        newModule.transform.position += correctiveTranslation;
    }


    private static TItem GetRandom<TItem>(TItem[] array)
    {
        int randomIndex = Random.Range(0, array.Length);
        return array[randomIndex];
    }

    private static ModuleConnector GetRandomExitWithTag(ModuleConnector[] moduleConnectors, string tagToMatch)
    {
        var matchingModuleConnectors = moduleConnectors.Where(m => m.CanReceive.Contains(tagToMatch)).ToArray();
        if (matchingModuleConnectors.Length == 0)
        {
            throw new System.Exception("No module connectors found with tag: " + tagToMatch);
        }
        return GetRandom(matchingModuleConnectors);
    }


    private static Module GetRandomWithTag(IEnumerable<Module> modules, string tagToMatch)
    {
        var matchingModules = modules.Where(m => m.Tags.Contains(tagToMatch)).ToArray();
        if (matchingModules.Length == 0)
        {
            throw new System.Exception("No modules found with tag: " + tagToMatch);
        }
        return GetRandom(matchingModules);
    }


    private static float Azimuth(Vector3 vector)
    {
        return Vector3.Angle(Vector3.forward, vector) * Mathf.Sign(vector.x);
    }

    void generateDebugCube(BoxCollider boxCollider, bool isRemoved = false)
    {
        Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.name = "BoxCollider";
        cube.transform.position = worldCenter;
        cube.transform.localScale = boxCollider.size;
        cube.transform.rotation = boxCollider.transform.rotation;

        if (isRemoved)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material = previewMaterial;
        }
    }

    void generateDebugSphere(SphereCollider sphereCollider, bool isRemoved = false)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        cube.transform.name = "SphereCollider";
        cube.transform.position = sphereCollider.bounds.center;
        cube.transform.localScale = new Vector3(sphereCollider.radius, sphereCollider.radius, sphereCollider.radius) * 2;

        if (isRemoved)
        {
            var cubeRenderer = cube.GetComponent<Renderer>();
            cubeRenderer.material = previewMaterial;
        }
    }

    bool colliderOverlapsOtherModule(Collider newModuleCollider)
    {
        if (newModuleCollider.GetComponent<BoxCollider>())
        {
            var boxCollider = newModuleCollider.GetComponent<BoxCollider>();

            Vector3 worldCenter = boxCollider.transform.TransformPoint(boxCollider.center);

            if (showOverlapColliders)
            {
                generateDebugCube(boxCollider);
            }

            Collider[] hitColliders = Physics.OverlapBox(worldCenter, boxCollider.size * 0.5f, boxCollider.transform.rotation);
            foreach (var hitCollider in hitColliders)
            {
                //If collision is of preparedModuleColliders
                if (hitCollider.gameObject.layer == 18)
                {
                    if (showRemovedOverlapColliders)
                    {
                        generateDebugCube(boxCollider, true);
                    }
                    //Debug.Log(newModule.name + " against " + hitCollider.transform.parent.parent.name);
                    return true;
                }
            }
        }

        if (newModuleCollider.GetComponent<SphereCollider>())
        {
            SphereCollider sphereCollider = newModuleCollider.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(sphereCollider.bounds.center, sphereCollider.radius);

            if (showOverlapColliders)
            {
                generateDebugSphere(sphereCollider);
            }

            foreach (var hitCollider in hitColliders)
            {

                if (hitCollider.gameObject.layer == 18)
                {
                    if (showRemovedOverlapColliders)
                    {
                        generateDebugSphere(sphereCollider, true);
                    }
                    //Debug.Log(newModule.name + " against " + hitCollider.name);
                    return true;
                }
            }
        }

        return false;
    }

    private void SpawnBlockade(ModuleConnector pendingExit)
    {
        var blockModule = (Module)Instantiate(BlockModule);
        var newModuleExits = blockModule.GetExits();
        var exitToMatch = newModuleExits.FirstOrDefault(x => x.IsDefault) ?? GetRandom(newModuleExits);
        ConnectExits(pendingExit, exitToMatch);
    }

    //This is a WIP fix for the players falling through the map it places a small collider between every doorway so people dont fall.
    private void SpawnThreshold(ModuleConnector pendingExit)
    {
        var threshold = (GameObject)Instantiate(Threshold);
        threshold.transform.rotation = pendingExit.transform.rotation;
        threshold.transform.position = pendingExit.transform.position;
        threshold.transform.parent = pendingExit.transform;
    }

    private void Reset() {
        currentModule = null;
        Module.lastModule = null;
    }
}
