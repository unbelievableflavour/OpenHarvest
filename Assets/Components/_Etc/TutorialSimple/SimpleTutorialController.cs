using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorialController : MonoBehaviour
{
    public GameObject tutorialObject;
    public Transform stepsParentObject;
    public MainTutorialMenuController mainTutorialMenuController;
    private List<GameObject> steps = new List<GameObject>();

    private int tutorialStep = 0;

    void Start()
    {
        foreach(Transform step in stepsParentObject)
        {
            steps.Add(step.gameObject);
        }
        LoadStep();
    }

    public void NextStep()
    {
        if(tutorialStep == steps.Count-1)
        {
            tutorialObject.SetActive(false);
            mainTutorialMenuController.ReturnToMainMenu();
            return;
        }
        tutorialStep++;
        LoadStep();
    }

    public void PreviousStep()
    {
        if (tutorialStep == 0)
        {
            return;
        }
        tutorialStep--;
        LoadStep();
    }

    public void StartOver()
    {
        tutorialStep = 0;
        LoadStep();
    }

    private void LoadStep()
    {
        DeactivateAllSteps();

        steps[tutorialStep].SetActive(true);

        Transform patch = steps[tutorialStep].transform.Find("Patch");
        if (patch && patch.GetComponent<ForcePlantState>())
        {
            patch.GetComponent<ForcePlantState>().SetState();
        }
    }

    private void DeactivateAllSteps()
    {
        foreach (GameObject step in steps)
        {
            step.SetActive(false);
        }
    }
}
