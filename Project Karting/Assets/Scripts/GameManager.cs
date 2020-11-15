using System;
using System.Collections;
using System.Collections.Generic;
using Kart;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public KartBase kartPrefab;
    [Range(1,10)] public int nbPlayerRacing = 1;
    public Transform[] spawnPoints;
    private PlayerRaceInfo[] playersInfo;

    private bool raceBegan;
    
    private void Update()
    {
        if (!raceBegan && Input.GetKeyDown("space"))
        {
            initRace();
            startRace();
        }
    }

    private void startRace()
    {
        
    }

    private void initRace()
    {
        if (spawnPoints.Length >= nbPlayerRacing)
        {
            for (int id = 0; id < nbPlayerRacing; ++id)
            {
                Transform spawn = spawnPoints[id];
                PlayerRaceInfo info = new PlayerRaceInfo();
                info.kart = Instantiate(kartPrefab, spawn.position, Quaternion.identity);
                info.playerId = id;
                info.lap = 1;
                info.position = id;
                info.currentCheckpoint = 0;
                playersInfo[id] = info;
            }
        }
    }
    public PlayerRaceInfo? getPlayerRaceInfo(int id)
    {
        foreach (var info in playersInfo)
        {
            if (info.playerId == id) return info;
        }

        return null;
    }
    
}
