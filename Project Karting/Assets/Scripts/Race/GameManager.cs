using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kart;
using Items;
using Player;
using RoadPhysics;

public class GameManager : MonoBehaviour {
    public KartBase kartPrefab;
    [Range(1, 10)] public int nbPlayerRacing = 1;
    [Range(1, 10)] public int nbLap = 1;
    public ItemManager itemManager;
    public int checkpointAmount;
    public Transform[] spawnPoints;

    [Header("UI and HUD")] [SerializeField]
    private GameObject HUDvsClockPrefab;

    [SerializeField] private GameObject StartUIPrefab;

    [Header("Debug")] public Text bestTime;
    public Text currentTime;
    public Text timeDiff;
    public Text lap;
    public Text checkpoint;
    public Text timeInfo;

    private PlayerRaceInfo[] playersInfo;

    private bool raceBegan;
    private bool raceIsInit;
    private StartMsgAnimation startMessage;

    private List<ShakeTransform> cameras;

    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [SerializeField] private PhysicsManager physicsManager;

    private GameManager() {
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            cameras = new List<ShakeTransform>();
            //Rest of your Awake code
        }
        else {
            Destroy(this);
        }

        physicsManager.Init();
        raceIsInit = false;
        InitRace();
    }

    private void Update() {
        if (raceBegan) {
            PlayerRaceInfo player = playersInfo[0];
            currentTime.text = floatToTimeString(Time.time - player.currentLapStartTime);
            lap.text = player.lap.ToString();
            checkpoint.text = player.currentCheckpoint.ToString();
            float diff = Time.time - player.currentLapStartTime;
            string info = "Time : " + floatToTimeString(Time.time) + "\nLap start time : " +
                          floatToTimeString(player.currentLapStartTime) + "\nDiff : " + floatToTimeString(diff);
            timeInfo.text = info;
            player.Controller.Update(); // listen player inputs 
        }
    }

    public void StartRace() {
        for (int i = 0; i < playersInfo.Length; ++i) {
            playersInfo[i].currentLapStartTime = Time.time;
            playersInfo[i].lap = 1;
        }

        raceBegan = true;
    }


    private void InitRace() {
        playersInfo = new PlayerRaceInfo[nbPlayerRacing];
        if (spawnPoints.Length >= nbPlayerRacing) {
            for (int id = 0; id < nbPlayerRacing; ++id) {
                Transform spawn = spawnPoints[id];
                KartBase kart = Instantiate(kartPrefab, spawn.position, spawn.rotation);


                PlayerRaceInfo
                    info = new PlayerRaceInfo(kart, id,
                        new PlayerAction()); //TODO : if human PlayerAction, if IA ComputerAction
                playersInfo[id] = info;
                Instantiate(HUDvsClockPrefab); // id automatically set inside the class
                startMessage = Instantiate(StartUIPrefab).GetComponentInChildren<StartMsgAnimation>();
                ShakeTransform cam = kart.cameraShake;
                if (cam != null) {
                    cameras.Add(cam);
                }

                startMessage._startTime = Time.time;
                raceIsInit = true;
            }
        }
        else {
#if UNITY_EDITOR
            Debug.LogError("Attempting to spawn " + nbPlayerRacing + " but only " + spawnPoints.Length +
                           " availables.");
#endif
        }
    }

    public PlayerRaceInfo getPlayerRaceInfo(int id) {
        foreach (var info in playersInfo) {
            if (info.playerId == id) return info;
        }

        return null;
    }

    public void checkpointPassed(int checkpointId, int playerId) {
        Debug.Log("Checkpoint passed");
        PlayerRaceInfo player = playersInfo[playerId];
        //permet de vérifier si premièrement le checkpoint est valide et si il est après le checkpoint actuel
        if (checkpointId < checkpointAmount) {
            if (checkpointId - player.currentCheckpoint == 1) {
                playersInfo[playerId].currentCheckpoint = checkpointId;
            }
            else if (player.currentCheckpoint == checkpointAmount - 1 && checkpointId == 0) {
                playersInfo[playerId].currentCheckpoint = checkpointId;
                newLap(playerId);
            }
        }

        //si le checkpoint validé est le dernier de la liste
    }

    private void newLap(int playerId) {
        Debug.Log("LAP");
        //on calcule le temps du lap
        playersInfo[playerId].previousLapTime = Time.time - playersInfo[playerId].currentLapStartTime;
        playersInfo[playerId].currentLapStartTime = Time.time;

        float diff = playersInfo[playerId].previousLapTime - playersInfo[playerId].bestLapTime;
        playersInfo[playerId].lap += 1; // doit être appelé ici pour mettre à jour la diff dans la HUD

        if (playersInfo[playerId].previousLapTime < playersInfo[playerId].bestLapTime) {
            Debug.Log("MEILLEUR TEMPS");
            playersInfo[playerId].bestLapTime = playersInfo[playerId].previousLapTime;
        }

        bestTime.text = floatToTimeString(playersInfo[playerId].bestLapTime);
        currentTime.text = floatToTimeString(playersInfo[playerId].previousLapTime);
        if (playersInfo[playerId].lap != 1) {
            timeDiff.text = floatToTimeString(diff);
            if (diff > 0) {
                timeDiff.color = Color.red;
            }
            else {
                timeDiff.color = Color.green;
            }
        }
    }

    private string floatToTimeString(float time) {
        string prefix = "";
        if (time < 0) prefix = "-";
        time = Mathf.Abs(time);
        int minutes = (int) time / 60;
        int seconds = (int) time - 60 * minutes;
        int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
        return prefix + string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    public bool raceHasBegan() {
        return raceBegan;
    }

    public void ShakeCameras(ShakeTransformEventData shake) {
        foreach (var cam in cameras) {
            cam.AddShakeEvent(shake);
        }
    }
}