using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RaceSelectionManager : MonoBehaviour
{
    public List<Race> races;
    public List<CarrousselSelector> raceSelectors;
    public TextMeshProUGUI raceName;
    public Animator goCanvasAnimator;
    public MenuManager menuManager;
    
    //private List<int> _selectedRaces;
    private int _currentRaceId;
    private int _currentCarrousselPos;

    public RaceSelectionManager()
    {
        //_selectedRaces = new List<int>();
    }

    private void Start()
    {
        for (int i = 0; i < raceSelectors.Count; i++)
        {
            CarrousselSelector selector = raceSelectors[i];
            UpdateSelector(i,raceSelectors[i]);
            selector.anim.SetInteger("position",i);
        }
        UpdateText();
    }

    public void ChoseRace()
    {
        //foreach (var raceId in _selectedRaces)
        //{
        Debug.Log("Choosing race");
            GameManager.Instance.gameConfig.races.Add(races[_currentRaceId]);
        //}
    }

    public void ShowNextRace()
    {
        int limit = races.Count;
        _currentRaceId = (_currentRaceId - 1 + limit) % limit;
        _currentCarrousselPos = (_currentCarrousselPos + 1 + 7) % 7;
        MoveSelectors();
        UpdateText();
    }
    public void ShowPreviousRace()
    {
        int limit = races.Count;
        _currentRaceId = (_currentRaceId + 1 + limit) % limit;
        _currentCarrousselPos = (_currentCarrousselPos - 1 + 7) % 7;
        MoveSelectors();
        UpdateText();
    }

    private void UpdateText()
    {
        raceName.text = races[_currentRaceId].name;
    }
    private void MoveSelectors()
    {
        string text = "CurrentId = " + _currentRaceId + "[ ";
        
        int carouselLimit = 7;
        for (int i = 0; i < raceSelectors.Count; i++)
        {
            Animator selector = raceSelectors[i].anim;
            int pos = ( i  + _currentCarrousselPos +carouselLimit) % carouselLimit;
            if (pos == 0 || pos == 6)
            {
                UpdateSelector(pos,raceSelectors[i]);
            }
            text += " " + pos;
            selector.SetInteger("position",pos);
        }
        
        text += " ]";
        Debug.Log(text);
    }
    private void UpdateSelector(int pos,CarrousselSelector selector)
    {
        Debug.Log("pos : " + pos + " _currentCarrousselPos : " + _currentCarrousselPos + " _currentRaceId : " + _currentRaceId);
        int offset = pos - 3; 
        Race associatedRace = races[(_currentRaceId + offset + races.Count) % races.Count];
        selector.image.sprite = associatedRace.image;
    }
    public void SelectRaces()
    {
        ChoseRace();
        goCanvasAnimator.SetBool("visible",true);
        menuManager.ShowNextScreen();
    }
}
