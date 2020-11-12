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

    private static GameManager _instance;

    public static GameManager Instance => _instance;

    private GameManager()
    {}

    private void Awake()
    {
        if (_instance == null){

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            //Rest of your Awake code

        } else {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (!raceBegan && Input.GetKeyDown("space"))
        {
            initRace();
            StartCoroutine(startRace());
        }
    }

    private IEnumerator startRace()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        Debug.Log("start of race");
        Debug.Log("3");
        yield return wait;
        Debug.Log("2");
        yield return wait;
        Debug.Log("1");
        yield return wait;
        Debug.Log("GO");
        raceBegan = true;
        
    }

    private void initRace()
    {
        Debug.Log("init of race");
        playersInfo = new PlayerRaceInfo[nbPlayerRacing];
        if (spawnPoints.Length >= nbPlayerRacing)
        {
            for (int id = 0; id < nbPlayerRacing; ++id)
            {
                Debug.Log("spawning kart " + id);
                Transform spawn = spawnPoints[id];
                KartBase kart = Instantiate(kartPrefab, spawn.position, spawn.rotation);
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

    public bool raceHasBegan()
    {
        return raceBegan;
    }
}
