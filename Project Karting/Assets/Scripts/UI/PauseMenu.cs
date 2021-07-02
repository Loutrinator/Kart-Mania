using System;
using System.Collections;
using System.Collections.Generic;
using Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI title;
    public List<GameObject> buttons;

    public void Start()
    {
        root.SetActive(false);
    }

    public void PauseGame()
    {
        root.SetActive(true);
    }
    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        root.SetActive(false);
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quitting the app !");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
    
    public void MainMenu()
    {
        ResumeGame();
        SceneManager.instance.LoadMainMenu();
    }
}
