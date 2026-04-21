using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;
    private Material cellIndicatorMaterialInstance;
    private bool isInitialized = false;

    private void Start()
    {
        EnsureInitialized();
    }

    public void StartShowingPlacementPreview(GameObject prefab)
    {
        EnsureInitialized();

        if (prefab == null)
        {
            return;
        }

        if (previewObject != null)
        {
            Destroy(previewObject);
        }

        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        if (cellIndicator != null)
        {
            cellIndicator.SetActive(true);
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        if (previewObject == null)
        {
            return;
        }

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
        {
                if (previewMaterialInstance != null)
                {
                    materials[i] = previewMaterialInstance;
                }
            }
            renderer.materials = materials;
        }

        Collider[] colliders = previewObject.GetComponentsInChildren<Collider>();
        foreach(Collider collider in colliders)
        {
            collider.enabled = false;
        }

        Rigidbody[] rigidbodies = previewObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            rigidbody.detectCollisions = false;
        }

        Joint[] joints = previewObject.GetComponentsInChildren<Joint>();
        foreach (Joint joint in joints)
        {
            joint.connectedBody = null;
            joint.enableCollision = false;
            Destroy(joint);
        }
    }

    public void StopShowingPreview()
    {
        if (cellIndicator != null)
        {
            cellIndicator.SetActive(false);
        }
        if(previewObject!= null) {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    public void UpdatePosition(Vector3 position, bool validity, Quaternion rotation)
    {
        EnsureInitialized();

        if(previewObject != null)
        {
            MovePreview(position, rotation);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        if (previewMaterialInstance == null)
        {
            return;
        }

        Color color = validity ? Color.white : Color.red;
        
        color.a = 0.5f;
        previewMaterialInstance.color = color;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        if (cellIndicatorMaterialInstance == null)
        {
            return;
        }

        Color color = validity ? Color.white : Color.red;

        color.a = 0.5f;
        cellIndicatorMaterialInstance.color = color;
    }

     private void MoveCursor(Vector3 position)
    {
        if (cellIndicator == null)
        {
            return;
        }

        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position, Quaternion rotation)
    {
        if (previewObject == null)
        {
            return;
        }

        previewObject.transform.position = new Vector3(
            position.x, 
            position.y + previewYOffset, 
            position.z);
        previewObject.transform.rotation = rotation;
    }

    private void EnsureInitialized()
    {
        if (isInitialized)
        {
            return;
        }

        if (previewMaterialPrefab != null)
        {
            previewMaterialInstance = new Material(previewMaterialPrefab);
        }

        if (cellIndicator != null)
        {
            cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
            if (cellIndicatorRenderer != null)
            {
                cellIndicatorMaterialInstance = cellIndicatorRenderer.material;
            }
            cellIndicator.SetActive(false);
        }

        isInitialized = true;
    }

}
