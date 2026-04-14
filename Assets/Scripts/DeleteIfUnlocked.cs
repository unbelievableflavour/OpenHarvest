using UnityEngine;

public class DeleteIfUnlocked : MonoBehaviour
{
    public string itemId;

    void Start()
    {
        if (GameState.Instance.isUnlocked(itemId))
        {
            Destroy(this.gameObject);
        }
    }
}
