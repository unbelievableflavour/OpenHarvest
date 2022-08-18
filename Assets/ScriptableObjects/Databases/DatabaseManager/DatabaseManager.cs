using HarvestDataTypes;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
	public ContractDatabase contracts;
	public ItemDatabase items;

	// Singleton instance.
	public static DatabaseManager Instance = null;

	// Initialize the singleton instance.
	private void Awake()
	{
		// If there is not already an instance of DatabaseManager, set it to this.
		if (Instance == null)
		{
			Instance = this;
		}
		//If an instance already exists, destroy whatever this object is to enforce the singleton.
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		//Set DatabaseManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
		DontDestroyOnLoad(gameObject);
	}
}
