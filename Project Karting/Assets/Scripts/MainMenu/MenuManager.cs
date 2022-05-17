using DG.Tweening;
using Game;
using Handlers;
using MainMenu;
using ProceduralAnimations;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{

    //[SerializeField] private Animator mainCameraAnimator;
    [SerializeField] private MainMenuCamera mainCamera;
    [SerializeField] private ProceduralAnimationData cameraAnimationData;
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
        /*soloAnimator.SetBool("NotSelected",true);
        multiAnimator.SetBool("Choosen",true);*/
        
        _eventSystem.SetSelectedGameObject(null);
        cameraAnimationData.PlayTransition(mainCamera.transform, "DeskToClipboard", () => {
            _eventSystem.SetSelectedGameObject(gamemodeCanvas.firstButton.gameObject);
        });
    }

    public void SelectSolo()
    {
        PlayerConfigurationManager.Instance.DisableJoining();
        SoundManager.Instance.PlayUIClick();
        /*soloAnimator.SetBool("Choosen",true);
        multiAnimator.SetBool("NotSelected",true);*/
        
        _eventSystem.SetSelectedGameObject(null);
        cameraAnimationData.PlayTransition(mainCamera.transform, "DeskToClipboard", () => {
            _eventSystem.SetSelectedGameObject(gamemodeCanvas.firstButton.gameObject);
        });
    }

    public void KartConfigsReady()
    {
        SoundManager.Instance.PlayUIClick();
        
        kartSelectorAnimator.SetBool("isHidden", true);
        
        _eventSystem.SetSelectedGameObject(null);
        var lightTvDelay = cameraAnimationData.transitions["CarToRaceSelection"].duration * 0.75f;
        DOVirtual.DelayedCall(lightTvDelay, () => mainCamera.SwitchTV(), false);
        cameraAnimationData.PlayTransition(mainCamera.transform, "CarToRaceSelection", () => {
            _eventSystem.SetSelectedGameObject(circuitCanvas.firstButton.gameObject);
        });
    }
    public void SelectRace()
    {
        SoundManager.Instance.PlayUIClick();
        _eventSystem.SetSelectedGameObject(null);
        cameraAnimationData.PlayTransition(mainCamera.transform, "ScreenToDoorGo", () => {
            _eventSystem.SetSelectedGameObject(goCanvas.firstButton.gameObject);
        });
    }
    public void SelectMode(int i)
    {
        PlayerConfigurationManager.Instance.DisableJoining();
        PlayerConfigurationManager.Instance.BeginSetup();
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

        //ShowNextScreen();
        SoundManager.Instance.PlayUIClick();
        _eventSystem.SetSelectedGameObject(null);
        cameraAnimationData.PlayTransition(mainCamera.transform, "ClipboardToCar", () => {
            _eventSystem.SetSelectedGameObject(kartCanvas.firstButton.gameObject);
        });
    }
    
    public void ShowPreviousScreen()
    {
        //mainCameraAnimator.SetTrigger("back"); todo
    }

    public void StartLevel() {
        _eventSystem.SetSelectedGameObject(null);
        SceneManager.instance.LoadGameMode(LevelManager.instance.gameConfig.mode);
    }
    public void QuitGame()
    {
        SoundManager.Instance.PlayUIBack();
        SceneManager.instance.QuitGame();
    }
    public void ShowCredits()
    {
        SoundManager.Instance.PlayUIClick();
        SceneManager.instance.LoadCredits();//TODO
    }
}
