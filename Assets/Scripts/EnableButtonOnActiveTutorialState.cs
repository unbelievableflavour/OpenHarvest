using UnityEngine;
using UnityEngine.UI;
using static Definitions;

public class EnableButtonOnActiveTutorialState : MonoBehaviour
{
    public TutorialStates tutorialStateThatShouldBeActive;

    private bool alreadyActive = false;

    void Update()
    {
        if (alreadyActive)
        {
            return;
        }

        if (InteractiveTutorialState.GetCurrentState() != tutorialStateThatShouldBeActive)
        {
            return;
        }

        var button = GetComponent<Button>();

        if (button)
        {
            alreadyActive = true;
            button.interactable = true;
        }
    }
}
