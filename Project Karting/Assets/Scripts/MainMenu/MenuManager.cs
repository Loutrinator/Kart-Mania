using System;
using Game;
using Handlers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Animator mainCameraAnimator;
    [SerializeField] private Animator soloAnimator;
    [SerializeField] private Animator multiAnimator;
    [SerializeField] private Animator levelEditorAnimator;
    [SerializeField] private Animator kartSelectorAnimator;
    [SerializeField] private CustomCanvas gamePlayerModeCanvas;
    [SerializeField] private UICanvas playermodeCanvas;
    [SerializeField] private UICanvas gamemodeCanvas;
    [SerializeField] private UICanvas kartCanvas;
    [SerializeField] private UICanvas circuitCanvas;
    [SerializeField] private UICanvas goCanvas;

    private EventSystem _eventSystem;
    
    public static MenuManager Instance { get; private set; }

    private void Awake()
    { 
        if (Instance != null)
        {
            Debug.Log("Trying to create another singleton");
        }
        else
        {
            Instance = this;
            SoundManager.Instance.PlayMainMenuMusic();
            _eventSystem = FindObjectOfType<EventSystem>();
            _eventSystem.SetSelectedGameObject(playermodeCanvas.firstButton.gameObject);
        }
    }

    public void SelectMultiplayer()
    {
        PlayerConfigurationManager.Instance.EnableJoining();
        PlayerConfigurationManager.Instance.multiplayer = true;
        SoundManager.Instance.PlayUIClick();
        soloAnimator.SetBool("NotSelected",true);
        multiAnimator.SetBool("Choosen",true);
        mainCameraAnimator.SetTrigger("move");
        _eventSystem.SetSelectedGameObject(gamemodeCanvas.firstButton.gameObject);
    }

    public void SelectSolo()
    {
        PlayerConfigurationManager.Instance.DisableJoining();
        SoundManager.Instance.PlayUIClick();
        soloAnimator.SetBool("Choosen",true);
        multiAnimator.SetBool("NotSelected",true);
        mainCameraAnimator.SetTrigger("move");
        _eventSystem.SetSelectedGameObject(gamemodeCanvas.firstButton.gameObject);
    }

    public void SelectKart()
    {
        ShowNextScreen();
        _eventSystem.SetSelectedGameObject(circuitCanvas.firstButton.gameObject);
        
    }
    public void SelectRace()
    {
        ShowNextScreen();
        _eventSystem.SetSelectedGameObject(goCanvas.firstButton.gameObject);
        
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
        _eventSystem.SetSelectedGameObject(kartCanvas.firstButton.gameObject);
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
