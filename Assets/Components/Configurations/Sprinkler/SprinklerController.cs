using UnityEngine;
using UnityEngine.UI;

public class SprinklerController : MonoBehaviour
{
    public GameObject errorMessage;
    public Text errorMessageValue;
    public ConfigurationModeToggler configurationController;

    void Start()
    {
        errorMessage.SetActive(false);
    }

    public void StartSprinklers()
    {
        if (configurationController.configurationModeIsActive())
        {
            CancelInvoke("HideErrorMessage");
            errorMessageValue.text = "Please disable configuration mode first";
            ShowErrorMessage();
            Invoke("HideErrorMessage", 1.0f);
            return;
        }

        int numberOfSprinklers = 0;
        foreach (var itemLocation in configurationController.itemLocations)
        {
            var sprinkler = itemLocation.options.GetChild(itemLocation.GetActiveIndex()).GetComponent<Sprinkler>();
            if (sprinkler)
            {
                numberOfSprinklers++;
                sprinkler.EnableSprinklers();
            }
        }

        if(numberOfSprinklers == 0)
        {
            CancelInvoke("HideErrorMessage");
            errorMessageValue.text = "No sprinklers configured";
            ShowErrorMessage();
            Invoke("HideErrorMessage", 1.0f);
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
