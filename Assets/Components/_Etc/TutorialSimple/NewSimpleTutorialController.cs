using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;

[System.Serializable]
public class TutorialDialogueButton
{
    public string text;
    public ButtonClickedEvent onClick;
}

[System.Serializable]
public class TutorialDialogue
{
    [TextArea(3, 10)] [SerializeField] public string text;

    //public QuestDialogueButton leftButton;
    //public QuestDialogueButton rightButton;
}
public class NewSimpleTutorialController : MonoBehaviour
{
    public MainTutorialMenuController mainTutorialMenuController;

    public Transform stepsParentObject;
    public GameObject stepObject;
    public TutorialDialogue[] dialogues;

    private int activeTutorialStepIndex = 0;

    void Start()
    {
        LoadStep();
    }

    public void NextStep()
    {
        if (activeTutorialStepIndex == dialogues.Length - 1)
        {
            gameObject.SetActive(false);
            mainTutorialMenuController.ReturnToMainMenu();
            return;
        }
        activeTutorialStepIndex++;
        LoadStep();
    }

    public void PreviousStep()
    {
        if (activeTutorialStepIndex == 0)
        {
            return;
        }
        activeTutorialStepIndex--;
        LoadStep();
    }

    public void StartOver()
    {
        activeTutorialStepIndex = 0;
        LoadStep();
    }

    private void LoadStep()
    {
        var currentStep = dialogues[activeTutorialStepIndex];
        var tutorialStep = stepObject.GetComponent<TutorialStep>();

        tutorialStep.text.text = currentStep.text;
        //tutorialStep.GetComponent<AutoType>().ResetText(currentStep.text);
        //tutorialStep.previousButton.onClick.AddListener(PreviousStep);
       // tutorialStep.nextButton.onClick.AddListener(NextStep);

        if (activeTutorialStepIndex == dialogues.Length - 1)
        {
            tutorialStep.finishButton.gameObject.SetActive(true);
            tutorialStep.startOverButton.gameObject.SetActive(true);

            tutorialStep.previousButton.gameObject.SetActive(false);
            tutorialStep.nextButton.gameObject.SetActive(false);
        } else
        {
            tutorialStep.finishButton.gameObject.SetActive(false);
            tutorialStep.startOverButton.gameObject.SetActive(false);

            tutorialStep.previousButton.gameObject.SetActive(activeTutorialStepIndex == 0 ? false : true);
            tutorialStep.nextButton.gameObject.SetActive(true);
        }
    }
}

