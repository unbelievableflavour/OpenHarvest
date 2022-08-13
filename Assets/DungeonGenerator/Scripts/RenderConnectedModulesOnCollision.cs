using UnityEngine;

public class RenderConnectedModulesOnCollision : MonoBehaviour
{
    public Module parentModule;

    private float elapsed = 0;
    private float delay = 0.5f;

    void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        if(ModularWorldGenerator.currentModule == parentModule)
        {
            return;
        }

        elapsed += Time.deltaTime;
        if (elapsed < delay)
        {
            return;
        }

        elapsed = 0;
        ModularWorldGenerator.currentModule = parentModule;
        parentModule.ToggleModule();
    }
}
