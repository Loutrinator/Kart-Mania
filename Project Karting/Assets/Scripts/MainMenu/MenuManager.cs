using System;
using Game;
using Handlers;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Animator mainCameraAnimator;
    [SerializeField] private Animator soloAnimator;
    [SerializeField] private Animator multiAnimator;
    [SerializeField] private Animator levelEditorAnimator;
    [SerializeField] private Animator kartSelectorAnimator;
    [SerializeField] private CustomCanvas gamePlayerModeCanvas;


    private void Awake()
    {
        SoundManager.Instance.PlayMainMenuMusic();
    }

    public void SelectMultiplayer()
    {
        PlayerConfigurationManager.Instance.multiplayer = true;
        SoundManager.Instance.PlayUIClick();
        soloAnimator.SetBool("NotSelected",true);
        multiAnimator.SetBool("Choosen",true);
        mainCameraAnimator.SetTrigger("move");
    }

    public void SelectSolo()
    {
        SoundManager.Instance.PlayUIClick();
        soloAnimator.SetBool("Choosen",true);
        multiAnimator.SetBool("NotSelected",true);
        mainCameraAnimator.SetTrigger("move");
    }
    public void SelectMode(int i)
    {
        PlayerConfigurationManager.Instance.DisableJoining();
        SoundManager.Instance.PlayUIClick();
        switch (i)
        {
            case 0://timetrial
                LevelManager.instance.gameConfig.mode = GameMode.TimeTrial;
                break;
            case 1://levelEditor
                LevelManager.instance.gameConfig.mode = GameMode.Editor;
                break;
            case 2://Championship
                LevelManager.instance.gameConfig.mode = GameMode.Championship;
                break;
            case 3://Championship
                LevelManager.instance.gameConfig.mode = GameMode.Versus;
                break;
        }
        mainCameraAnimator.SetTrigger("move");
        
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
