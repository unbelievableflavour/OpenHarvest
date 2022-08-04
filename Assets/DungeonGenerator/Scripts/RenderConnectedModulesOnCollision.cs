using UnityEngine;

public class RenderConnectedModulesOnCollision : MonoBehaviour
{
    public Module parentModule;

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

        ModularWorldGenerator.currentModule = parentModule;
        parentModule.ToggleModule();
    }
}
