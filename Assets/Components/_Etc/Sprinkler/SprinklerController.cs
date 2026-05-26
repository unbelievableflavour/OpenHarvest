using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprinklerController : MonoBehaviour
{
    private const string SPRINKLER_UNLOCKABLE_ID = "SprinklerStandard";

    public GameObject errorMessage;
    public Text errorMessageValue;
    public List<Sprinkler> sprinklers;

    void Start()
    {
        errorMessage.SetActive(false);
    }

    public void StartSprinklers()
    {
        if (!PlayerOwnsAtLeastOneSprinkler())
        {
            CancelInvoke("HideErrorMessage");
            errorMessageValue.text = "No sprinklers unlocked";
            ShowErrorMessage();
            Invoke("HideErrorMessage", 1.0f);
            return;
        }

        int numberOfSprinklers = 0;
        foreach (var sprinkler in GetSprinklers())
        {
            numberOfSprinklers++;
            sprinkler.EnableSprinklers();
        }

        if(numberOfSprinklers == 0)
        {
            CancelInvoke("HideErrorMessage");
            errorMessageValue.text = "No sprinklers configured";
            ShowErrorMessage();
            Invoke("HideErrorMessage", 1.0f);
        }
    }

    private bool PlayerOwnsAtLeastOneSprinkler()
    {
        return GameState.Instance.isUnlocked(SPRINKLER_UNLOCKABLE_ID);
    }

    private IEnumerable<Sprinkler> GetSprinklers()
    {
        foreach (var sprinkler in sprinklers)
        {
            yield return sprinkler;
        }
    }

    void ShowErrorMessage()
    {
        errorMessage.SetActive(true);
    }

    void HideErrorMessage()
    {
        errorMessage.SetActive(false);
    }
}
