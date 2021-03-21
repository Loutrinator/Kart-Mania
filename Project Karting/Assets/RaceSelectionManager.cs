using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceSelectionManager : MonoBehaviour
{
    public List<Race> races;
    private int _currentRaceId;
    private List<int> _selectedRaces;
    public List<Animator> raceSelectors;

    public RaceSelectionManager()
    {
        _selectedRaces = new List<int>();
    }

    private void Start()
    {
        for (int i = 0; i < raceSelectors.Count; i++)
        {
            Animator selector = raceSelectors[i];
            selector.SetInteger("position",i);
        }
    }

    public void ChoseRace()
    {
        foreach (var raceId in _selectedRaces)
        {
            GameManager.Instance.gameConfig.races.Append(races[raceId]);
        }
    }

    public void ShowNextRace()
    {
        int limit = races.Count;
        _currentRaceId = (_currentRaceId + 1 + limit) % limit;
        UpdateSelectors();
    }
    public void ShowPreviousRace()
    {
        int limit = races.Count;
        _currentRaceId = (_currentRaceId - 1 + limit) % limit;
        UpdateSelectors();
    }

    private void UpdateSelectors()
    {
        int carouselLimit = 7;
        for (int i = 0; i < raceSelectors.Count; i++)
        {
            Animator selector = raceSelectors[i];
            int pos = ( i - 3 + _currentRaceId +carouselLimit) % carouselLimit;
            selector.SetInteger("position",pos);
        }
    }
}
