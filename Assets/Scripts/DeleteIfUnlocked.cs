using UnityEngine;

public class DeleteIfUnlocked : MonoBehaviour
{
    public string itemId;

    void Start()
    {
        if (GameState.isUnlocked(itemId))
        {
            Destroy(this.gameObject);
        }
    }
}
