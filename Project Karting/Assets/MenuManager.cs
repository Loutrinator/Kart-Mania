using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public enum GameMode {timeTrial,championship,levelEditor}

    [SerializeField] private Animator mainCameraAnimator;
    [SerializeField] private Animator logoAnimator;
    [SerializeField] private Animator timeTrialAnimator;
    [SerializeField] private Animator championshipAnimator;
    [SerializeField] private Animator levelEditorAnimator;
    [SerializeField] private CustomCanvas gameModeCanvas;
    [SerializeField] private int vehiculeAmount;
    private int vehiculeId;
    private GameMode gameMode;
    
    public void SelectMode(GameMode mode)
    {
        gameMode = mode;
        logoAnimator.SetBool("isVisible", false);
        mainCameraAnimator.SetTrigger("move");
        gameModeCanvas.disableUIInteraction();
    }
    public void SelectModeTimeTrial()
    {
        timeTrialAnimator.SetBool("Choosen",true);
        levelEditorAnimator.SetBool("NotSelected",true);
        championshipAnimator.SetBool("NotSelected",true);
        SelectMode(GameMode.timeTrial);
    }
    public void SelectModeLevelEditor()
    {
        timeTrialAnimator.SetBool("NotSelected",true);
        levelEditorAnimator.SetBool("Choosen",true);
        championshipAnimator.SetBool("NotSelected",true);
        SelectMode(GameMode.levelEditor);
    }
    public void SelectModeChampionship()
    {
        timeTrialAnimator.SetBool("NotSelected",true);
        levelEditorAnimator.SetBool("NotSelected",true);
        championshipAnimator.SetBool("Choosen",true);
        SelectMode(GameMode.championship);
    }
    public void SelectKart(int id)
    {
        vehiculeId = (vehiculeId + 1) % vehiculeAmount;
        Debug.Log("Showing kart n°" + vehiculeId);
    }
    public void ShowPreviousScreen()
    {
        mainCameraAnimator.SetTrigger("back");
    }
    
}
