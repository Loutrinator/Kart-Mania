using System;
using System.Collections;
using System.Collections.Generic;
using Handlers;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Animator mainCameraAnimator;
    [SerializeField] private Animator logoAnimator;
    [SerializeField] private Animator timeTrialAnimator;
    [SerializeField] private Animator championshipAnimator;
    [SerializeField] private Animator levelEditorAnimator;
    [SerializeField] private Animator kartSelectorAnimator;
    [SerializeField] private CustomCanvas gameModeCanvas;
    [SerializeField] private TransitionController transitionController;


    private void Start()
    {
        logoAnimator.SetBool("isVisible", true);
    }

    public void SelectModeTimeTrial()
    {
        timeTrialAnimator.SetBool("Choosen",true);
        levelEditorAnimator.SetBool("NotSelected",true);
        championshipAnimator.SetBool("NotSelected",true);
        logoAnimator.SetBool("isVisible", false);
        mainCameraAnimator.SetTrigger("move");
        gameModeCanvas.disableUIInteraction();
        GameManager.Instance.gameConfig.mode = GameMode.timeTrial;
    }
    public void SelectModeLevelEditor()
    {
        timeTrialAnimator.SetBool("NotSelected",true);
        levelEditorAnimator.SetBool("Choosen",true);
        championshipAnimator.SetBool("NotSelected",true);
        logoAnimator.SetBool("isVisible", false);
        mainCameraAnimator.SetTrigger("move");
        gameModeCanvas.disableUIInteraction();
        GameManager.Instance.gameConfig.mode = GameMode.editor;
    }
    public void SelectModeChampionship()
    {
        timeTrialAnimator.SetBool("NotSelected",true);
        levelEditorAnimator.SetBool("NotSelected",true);
        championshipAnimator.SetBool("Choosen",true);
        logoAnimator.SetBool("isVisible", false);
        mainCameraAnimator.SetTrigger("move");
        gameModeCanvas.disableUIInteraction();
        GameManager.Instance.gameConfig.mode = GameMode.championship;
    }
    public void ShowNextScreen()
    {
        mainCameraAnimator.SetTrigger("move");
    }
    public void ShowPreviousScreen()
    {
        mainCameraAnimator.SetTrigger("back");
    }
    public void HideKartSelector()
    {
        Debug.Log("HIDE");
        kartSelectorAnimator.SetBool("isHidden", true);
    }

    public void ShowTransition()
    {
        Debug.Log("ShowTransition");
        transitionController.FadeIn();
    }
}
