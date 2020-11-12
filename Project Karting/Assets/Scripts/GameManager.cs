using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public KartBase kartPrefab;
    [Range(1,10)] public int nbPlayerRacing = 1;
    public Transform[] spawnPoints;
    private PlayerRaceInfo[] playersInfo;

    private bool raceBegan;

    public static GameManager instance;

    private GameManager()
    {}


    private void Start()
    {
        DontDestroyOnLoad(this);
    }

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
        raceBegan = true;
    }

    private void initRace()
    {
        if (spawnPoints.Length >= nbPlayerRacing)
        {
            for (int id = 0; id < nbPlayerRacing; ++id)
            {
                Transform spawn = spawnPoints[id];
                KartBase kart = Instantiate(kartPrefab, spawn.position, Quaternion.identity);
                PlayerRaceInfo info = new PlayerRaceInfo(kart, id);
                playersInfo[id] = info;
            }
        }
        else
        {
            Debug.LogError("Attempting to spawn " + nbPlayerRacing + " but only " + spawnPoints.Length + " availables.");
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

    public void checkpointPassed(int checkpointId, int playerId)
    {
        PlayerRaceInfo player = playersInfo[playerId];
        player.lap += 1;
        player.previousLapTime = Time.time - player.currentLapStartTime;
        if (player.previousLapTime < player.bestLapTime)
        {
            player.bestLapTime = player.previousLapTime;
            //call d'effets sur la HUD du joueur "nouveau meilleur score !"
        }
    }
    
}
