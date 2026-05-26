using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Sits on the pet prefab. Receives its plateauInstanceId from the plateau on spawn
/// so it can look up its own Animal data from GameState directly, with no dependency
/// on the plateau hierarchy.
/// </summary>
public class DetermineModelByAge : MonoBehaviour
{
    public GameObject pet;
    public GameObject petYoung;

    [HideInInspector]
    public string plateauInstanceId;

    public void SetId(string id)
    {
        plateauInstanceId = id;
        Refresh();
    }

    void Start()
    {
        Refresh();
        TimeController.Instance?.ListenToDayChange(OnDayChanged);
    }

    void OnDestroy()
    {
        TimeController.Instance?.RemoveFromDayChange(OnDayChanged);
    }

    private void OnDayChanged(object sender, EventArgs e)
    {
        Refresh();
    }

    void Refresh()
    {
        Animal animal = GameState.Instance?.animals?.FirstOrDefault(a => a.plateauInstanceId == plateauInstanceId);

        bool isDead = animal != null &&
                      ((TimeSpan)(TimeController.getCurrentTime() - animal.lastTimeFedTimestamp)).Days >= 10;

        if (animal == null || isDead)
        {
            Destroy(gameObject);
            return;
        }

        bool isAdult = ((TimeSpan)(TimeController.getCurrentTime() - animal.bornTimestamp)).Days >= 10;
        pet.SetActive(isAdult);
        petYoung.SetActive(!isAdult);
    }
}
