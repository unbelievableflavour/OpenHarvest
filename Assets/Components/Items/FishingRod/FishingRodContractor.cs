using UnityEngine;
using UnityEngine.UI;

public class FishingRodContractor : MonoBehaviour
{
    public GameObject[] fishingRodParts;
    public Transform[] LinePositions;
    public GameObject connectedPoint;

    public void ShowParts(int numberOfParts)
    {
        ConnectToPart(numberOfParts);

        DisableAllParts();

        int i = 0;
        foreach (GameObject part in fishingRodParts)
        {
            if(i >= numberOfParts)
            {
                return;
            }

            part.SetActive(true);
            i++;
        }
    }

    private void DisableAllParts ()
    {
        foreach (GameObject part in fishingRodParts)
        {
            part.SetActive(false);
        }
    }

    private void ConnectToPart(int numberOfParts)
    {
        var newlineStart = LinePositions[numberOfParts-1];
        connectedPoint.transform.position = newlineStart.position;
    }
}
