using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropColliderController : MonoBehaviour
{
    private WateringCanController wateringCanController;

    public void SetWateringCanController(WateringCanController newWateringCanController)
    {
        wateringCanController = newWateringCanController;
    }

    public void NotifyWateringCan()
    {
        wateringCanController.DecreaseWaterAmount();
    }
}
