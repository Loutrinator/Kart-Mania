using System;
using Game;
using Handlers;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Animator mainCameraAnimator;
    [SerializeField] private Animator timeTrialAnimator;
    [SerializeField] private Animator championshipAnimator;
    [SerializeField] private Animator levelEditorAnimator;
    [SerializeField] private Animator kartSelectorAnimator;
    [SerializeField] private CustomCanvas gameModeCanvas;


    private void Awake()
    {
        SoundManager.Instance.PlayMainMenuMusic();
    }

    public void SelectModeTimeTrial()
    {
        SoundManager.Instance.PlayUIClick();
        timeTrialAnimator.SetBool("Choosen",true);
        levelEditorAnimator.SetBool("NotSelected",true);
        championshipAnimator.SetBool("NotSelected",true);
        mainCameraAnimator.SetTrigger("move");
        gameModeCanvas.disableUIInteraction();
        LevelManager.instance.gameConfig.mode = GameMode.TimeTrial;
    }
    public void SelectModeLevelEditor()
    {
        SoundManager.Instance.PlayUIClick();
        timeTrialAnimator.SetBool("NotSelected",true);
        levelEditorAnimator.SetBool("Choosen",true);
        championshipAnimator.SetBool("NotSelected",true);
        mainCameraAnimator.SetTrigger("move");
        gameModeCanvas.disableUIInteraction();
        LevelManager.instance.gameConfig.mode = GameMode.Editor;
    }
    public void SelectModeChampionship()
    {
        SoundManager.Instance.PlayUIClick();
        timeTrialAnimator.SetBool("NotSelected",true);
        levelEditorAnimator.SetBool("NotSelected",true);
        championshipAnimator.SetBool("Choosen",true);
        mainCameraAnimator.SetTrigger("move");
        gameModeCanvas.disableUIInteraction();
        LevelManager.instance.gameConfig.mode = GameMode.Championship;
    }
    public void ShowNextScreen()
    {
        SoundManager.Instance.PlayUIClick();
        mainCameraAnimator.SetTrigger("move");
    }
    public void ShowPreviousScreen()
    {
        mainCameraAnimator.SetTrigger("back");
    }
    public void HideKartSelector()
    {
        kartSelectorAnimator.SetBool("isHidden", true);
    }

    public void ShowTransition()
    {
        TransitionController.Instance.FadeIn(StartLevel);//TODO
    }

    private void StartLevel() {
        SceneManager.instance.LoadGameMode(LevelManager.instance.gameConfig.mode);//TODO
    }
    public void QuitGame()
    {
        SoundManager.Instance.PlayUIBack();
        GameManager.Instance.QuitGame();
    }
    public void ShowCredits()
    {
        SoundManager.Instance.PlayUIClick();
        SceneManager.instance.LoadCredits();//TODO
    }
}
