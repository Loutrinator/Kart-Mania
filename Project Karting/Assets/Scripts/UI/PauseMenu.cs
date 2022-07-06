using System;
using System.Collections;
using System.Collections.Generic;
using Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI title;
    public List<GameObject> buttons;

    private PlayerConfiguration _playerConfigurationPaused;

    public void Start()
    {
        root.SetActive(false);
    }

    public void PauseGame(PlayerConfiguration playerConfiguration) {
        _playerConfigurationPaused = playerConfiguration;
        _playerConfigurationPaused.Input.SwitchCurrentActionMap("UI");
        root.SetActive(true);
        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }
    public void ResumeGame()
    {
        _playerConfigurationPaused?.Input.SwitchCurrentActionMap("Kart");
        SoundManager.Instance.PlayUIBack();
        GameManager.Instance.ResumeGame();
        EventSystem.current.SetSelectedGameObject(null);
        root.SetActive(false);
    }
    
    public void QuitGame()
    {
        SoundManager.Instance.PlayUIClick();
#if UNITY_EDITOR
        Debug.Log("Quitting the app !");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    public void MainMenu()
    {
        SoundManager.Instance.PlayUIClick();
        LevelManager.instance.OnRaceQuit(_playerConfigurationPaused);
        _playerConfigurationPaused = null;
        ResumeGame();
        TransitionController.Instance.FadeIn(() =>
            SceneManager.instance.LoadMainMenu(() => {
                TransitionController.Instance.FadeOut();
                PlayerConfigurationManager.Instance.EnableJoining();
            }));
    }
}
