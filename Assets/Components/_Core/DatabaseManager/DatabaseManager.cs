using HarvestDataTypes;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
	public ContractDatabase contracts;
	public ItemDatabase items;

	public static DatabaseManager Instance = null;

    // Initialize instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
