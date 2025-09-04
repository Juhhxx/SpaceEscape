using DG.Tweening;
using UnityEngine;

public class MainMenuTransition : MonoBehaviour
{
    [Header("UI Views")]
    [SerializeField] private GameObject mainMenuView;
    [SerializeField] private GameObject customizationView;
    [SerializeField] private GameObject missionView;
    [SerializeField] private GameObject winView;
    [SerializeField] private GameObject loseView;

    [Header("Transition")]
    [SerializeField] private GateTransition transition;

    private GameObject currentObject;

    private bool isStartingGame = false;

    void Start()
    {
        // Set the initial screen
        currentObject = mainMenuView;
        ShowOnly(mainMenuView);
    }

    public void TransitionToCustomization()
    {
        if(isStartingGame) return; // Prevent starting if cooldown is active
        isStartingGame = true; // Activate cooldown
        transition.TriggerTransition(currentObject, customizationView);
        currentObject = customizationView;


        // Reset cooldown after 3 seconds
        DOVirtual.DelayedCall(3f, () => isStartingGame = false);
    }

    public void TransitionToMission()
    {
        if (isStartingGame) return; // Prevent starting if cooldown is active
        isStartingGame = true; // Activate cooldown
        transition.TriggerTransition(currentObject, missionView);
        currentObject = missionView;


        // Reset cooldown after 3 seconds
        DOVirtual.DelayedCall(3f, () => isStartingGame = false);
    }

    public void TransitionToWin()
    {
        if(isStartingGame) return; // Prevent starting if cooldown is active
        isStartingGame = true; // Activate cooldown
        transition.TriggerTransition(currentObject, winView);
        currentObject = winView;


        // Reset cooldown after 3 seconds
        DOVirtual.DelayedCall(3f, () => isStartingGame = false);
    }

    public void TransitionToLose()
    {
        if(isStartingGame) return; // Prevent starting if cooldown is active
        isStartingGame = true; // Activate cooldown
        transition.TriggerTransition(currentObject, loseView);
        currentObject = loseView;


        // Reset cooldown after 3 seconds
        DOVirtual.DelayedCall(3f, () => isStartingGame = false);
    }

    public void TransitionToMainMenu()
    {
        if(isStartingGame) return; // Prevent starting if cooldown is active
        isStartingGame = true; // Activate cooldown
        transition.TriggerTransition(currentObject, mainMenuView);
        currentObject = mainMenuView;


        // Reset cooldown after 3 seconds
        DOVirtual.DelayedCall(3f, () => isStartingGame = false);
    }

    private void ShowOnly(GameObject target)
    {
        // Disable all views except the target one
        mainMenuView.SetActive(false);
        customizationView.SetActive(false);
        missionView.SetActive(false);
        winView.SetActive(false);
        loseView.SetActive(false);

        target.SetActive(true);
    }
}
