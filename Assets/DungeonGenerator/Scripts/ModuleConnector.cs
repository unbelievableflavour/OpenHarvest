using UnityEngine;


public class ModuleConnector : MonoBehaviour
{
	public Module parentModule = null;
	public string[] CanReceive;
	public string[] CanSpawn;
	public bool IsDefault;

	[Header("RoomConnector-only options")]
	public bool needsBlockade = false;
	public int spawnRate = 100;

	[ReadOnly]
	public ModuleConnector connectedModuleConnector = null;

	void OnDrawGizmos()
	{
		var scale = 1.0f;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * scale);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position - transform.right * scale);
		Gizmos.DrawLine(transform.position, transform.position + transform.right * scale);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + Vector3.up * scale);

		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.position, 0.125f);
	}

	public Module GetModule()
	{
		return parentModule;
	}
}
