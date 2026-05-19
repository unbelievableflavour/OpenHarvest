using UnityEngine;

/// <summary>
/// Attached to every placed object by <see cref="PlacementSystem"/>.
/// Holds a stable GUID generated once at placement time and persisted in the save file,
/// so plateau components can identify which Animal in GameState belongs to them.
/// </summary>
public class PlacedObjectInstanceId : MonoBehaviour
{
    public string instanceId;
}
