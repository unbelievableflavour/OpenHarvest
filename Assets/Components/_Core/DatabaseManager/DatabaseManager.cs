using HarvestDataTypes;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
	public ContractDatabase contracts;
	public ItemDatabase items;
    public QuestDatabase quests;
    public NpcDatabase npcs;
    public PlaceableObjectDatabase placeableObjects;
    public UnlockableDatabase unlockables;
    public SceneDatabase scenes;

	public static DatabaseManager Instance = null;

    // Initialize instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
