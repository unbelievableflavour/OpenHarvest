using UnityEngine;

public class Milkable : MonoBehaviour
{
    public GameObject milkingParticles;
    public MilkingAreaController milkingAreaController;

    public void StartMilking()
    {
        milkingParticles.SetActive(true);
        Invoke("StopMilking", 1);
        milkingAreaController.Pump();
    }

    void StopMilking()
    {
        milkingParticles.SetActive(false);
    }
}
