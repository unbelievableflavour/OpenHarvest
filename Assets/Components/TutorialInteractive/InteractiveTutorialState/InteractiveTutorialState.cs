using UnityEngine;
using static Definitions;

public class InteractiveTutorialState : MonoBehaviour
{
    private static TutorialStates currentState;

    public void GrabBackpack()
    {
        currentState = TutorialStates.GrabbedBackpack;
    }

    public void GrabWallet()
    {
        currentState = TutorialStates.GrabbedWallet;
    }

    public void EnabledTooltip()
    {
        currentState = TutorialStates.EnabledTooltip;
    }

    public void Shoveled()
    {
        currentState = TutorialStates.Shoveled;
    }

    public void Hoed()
    {
        currentState = TutorialStates.Hoed;
    }

    public void Seeded()
    {
        currentState = TutorialStates.Seeded;
    }

    public void Watered()
    {
        currentState = TutorialStates.Watered;
    }

    public void Reset()
    {
        currentState = TutorialStates.Initialized;
    }

    public static TutorialStates GetCurrentState()
    {
        return currentState;
    }
}
