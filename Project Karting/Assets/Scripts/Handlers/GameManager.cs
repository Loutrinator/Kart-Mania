using System.Collections.Generic;
using Game;
using Items;
using Kart;
using Player;
using Road.RoadPhysics;
using UI;
using UnityEngine;

namespace Handlers {
    public enum GameState
    {
        idle,start,race, finish
    }
    public class GameManager : MonoBehaviour
    {
        
        public GameState gameState;
        public Race currentRace;
        public Minimap minimap;

        public ItemManager itemManager;
        public int checkpointAmount;

        [Header("UI and HUD")]
        [SerializeField] private HUDTimeTrialController HUDvsClockPrefab;
        [SerializeField] private ScoreBoard ScoreBoardPrefab;

        [SerializeField] private GameObject StartUIPrefab;

        [HideInInspector] public List<KartBase> karts = new List<KartBase>();

        private PlayerRaceInfo[] playersInfo;

        private StartMsgAnimation startMessage;
        private float startTime;

        //private List<ShakeTransform> cameras;
        public CameraFollowPlayer cameraParentPrefab;

        public static GameManager Instance { get; private set; }

        public PhysicsManager physicsManager;
        public KartRespawner respawner;

        [HideInInspector]
        public PauseMenu pauseMenu;

        private bool gamePaused;
        
        private void Awake()
        {
            if (Instance == null) {
                Instance = this;
                //cameras = new List<ShakeTransform>();
            }
            else {
                Destroy(gameObject);
            }

            currentRace = LevelManager.instance.InitLevel();
            minimap.race = currentRace;
            minimap.DrawMinimap();

            if (physicsManager != null)
            {
                physicsManager.Init(currentRace.road.bezierSpline);
            }
            
            InitRace();

            if (respawner != null)
            {
                respawner.Init();
            }
            
            TransitionController.Instance.FadeOut(() => {
                //raceBegan = true;  // todo enable after delay
            });

            gamePaused = false;
            pauseMenu = FindObjectOfType<PauseMenu>();
        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("PAUSE");
                gamePaused = !gamePaused;
                if (gamePaused)
                {
                    Time.timeScale = 0;
                    pauseMenu.PauseGame();
                }
                else
                {
                    Time.timeScale = 1;
                    pauseMenu.ResumeGame();
                }
            }
            if (gameState == GameState.race) {
                PlayerRaceInfo player = playersInfo[0];
                //currentTime.text = floatToTimeString(Time.time - player.currentLapStartTime);
                //lap.text = player.lap.ToString();
                //checkpoint.text = player.currentCheckpoint.ToString();
                //float diff = Time.time - player.currentLapStartTime;
                // info = "Time : " + floatToTimeString(Time.time) + "\nLap start time : " +
                //              floatToTimeString(player.currentLapStartTime) + "\nDiff : " + floatToTimeString(diff);
                //timeInfo.text = info;
                player.Controller.Update(); // listen player inputs 
            }
            minimap.UpdateMinimap();
        }

        public void StartRace() {
            for (int i = 0; i < playersInfo.Length; ++i) {
                playersInfo[i].currentLapStartTime = Time.time;
                playersInfo[i].lap = 1;
            }

            gameState = GameState.race;
            foreach (var kart in karts)
            {
                kart.effects.StopRewind();
            }
            
        }

        private void InitRace()
        {
            gameState = GameState.start;
            int nbPlayerRacing = LevelManager.instance.gameConfig.players.Count;
            playersInfo = new PlayerRaceInfo[nbPlayerRacing];
            Transform[] spawnPoints = currentRace.spawnPoints;
            if (spawnPoints.Length >= nbPlayerRacing) {
                for (int id = 0; id < nbPlayerRacing; ++id) {
                    PlayerConfig playerConfig = LevelManager.instance.gameConfig.players[id];
                    Transform spawn = spawnPoints[id];
                    KartBase kart = Instantiate(playerConfig.kartPrefab, spawn.position, spawn.rotation);
                    KartEffects kartEffects = kart.GetComponent<KartEffects>();
                    KartAudio kartAudio = kart.GetComponent<KartAudio>();
                    
                    
                    // cameras for players
                    var kartCam = Instantiate(cameraParentPrefab, kart.transform.position, kart.transform.rotation);
                    kartCam.target = kart.transform;
                    if (kartEffects != null)
                    {
                        kartEffects.cameraShakeTransform = kartCam.cameraShakeTransform;
                        kartEffects.cam = kartCam.frontCamera;
                    }
                    if (kartAudio != null)
                    {
                        kartAudio.cam = kartCam.frontCamera;
                    }
                    karts.Add(kart);
                    
                    PlayerRaceInfo info = new PlayerRaceInfo(kart, id, new PlayerAction()); //TODO : if human PlayerAction, if IA ComputerAction
                    info.camera = kartCam;
                    playersInfo[id] = info;
                    
                    Instantiate(HUDvsClockPrefab); // id automatically set inside the class
                    startMessage = Instantiate(StartUIPrefab).GetComponentInChildren<StartMsgAnimation>();
                    ShakeTransform cam = kart.cameraShake;
                    
                    startTime = startMessage.getStartTime();
                    StartMsgAnimation startUI = startMessage.GetComponent<StartMsgAnimation>();
                    if (startUI != null)
                    {
                        startUI.placeKeysAction = placeKeys;
                    }
                    minimap.AddVisualObject(kart.gameObject, kart.minimapRenderer);

                    gameState = GameState.start;
                }
            } else {
#if UNITY_EDITOR
                Debug.LogError("Attempting to spawn " + nbPlayerRacing + " but only " + spawnPoints.Length + " available.");
#endif
            }
        }

        private void placeKeys()
        {
            foreach (var kart in karts)
            {
                kart.effects.InsertKey();
            }
        }
        public PlayerRaceInfo GetPlayerRaceInfo(int id) {
            foreach (var info in playersInfo) {
                if (info.playerId == id) return info;
            }

            return null;
        }

        public void CheckpointPassed(int checkpointId, int playerId) {
            PlayerRaceInfo player = playersInfo[playerId];

            //permet de vérifier si premièrement le checkpoint est valide et si il est après le checkpoint actuel
            if (checkpointId < checkpointAmount) {
                if (checkpointId - player.currentCheckpoint == 1) {
                    playersInfo[playerId].currentCheckpoint = checkpointId;
                }
                else if (player.currentCheckpoint == checkpointAmount - 1 && checkpointId == 0) {
                    playersInfo[playerId].currentCheckpoint = checkpointId;
                    NewLap(playerId);
                }
            }

            //si le checkpoint validé est le dernier de la liste
        }

        private void NewLap(int playerId) {
            //on calcule le temps du lap
            playersInfo[playerId].previousLapTime = Time.time - playersInfo[playerId].currentLapStartTime;
            playersInfo[playerId].currentLapStartTime = Time.time;

            float diff = playersInfo[playerId].previousLapTime - playersInfo[playerId].bestLapTime;
            playersInfo[playerId].lap += 1; // doit être appelé ici pour mettre à jour la diff dans la HUD
            playersInfo[playerId].lapsTime.Add( playersInfo[playerId].previousLapTime); //doit être appelé ici pour être sur que le previousLapTime est à jour
            
            //TODO : il y a un problème, la liste n'est pas bien conservé car à l'affichage du score
            // board de fin de course, il ne reste que le dernier temps dans la liste
            foreach (var t in  playersInfo[playerId].lapsTime)
            {
                Debug.Log("time add " + Utils.DisplayHelper.floatToTimeString(t));   
            }
            if (playersInfo[playerId].previousLapTime < playersInfo[playerId].bestLapTime) {
                playersInfo[playerId].bestLapTime = playersInfo[playerId].previousLapTime;
            }

            /*bestTime.text = floatToTimeString(playersInfo[playerId].bestLapTime);
        currentTime.text = floatToTimeString(playersInfo[playerId].previousLapTime);
        if (playersInfo[playerId].lap != 1) {
            timeDiff.text = floatToTimeString(diff);
            if (diff > 0) {
                timeDiff.color = Color.red;
            }
            else {
                timeDiff.color = Color.green;
            }
        }*/
            
            // TODO : il serait préférable de terminé le race pour ce playerID
            // mais de finir pour la totalité des participants selon une autre condition
            // quand on sera en écran splitté (cf.  document de game design pour la condition de fin de course)

            if (playersInfo[playerId].lap >= currentRace.laps)
            {
                playersInfo[playerId].FinishRace();
                FinishRace(playerId);
            }
        }

        private string FloatToTimeString(float time) {
            string prefix = "";
            if (time < 0) prefix = "-";
            time = Mathf.Abs(time);
            int minutes = (int) time / 60;
            int seconds = (int) time - 60 * minutes;
            int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
            return prefix + string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }

        public bool RaceHadBegun() {
            return  (gameState == GameState.race);
        }

        public void ShakeCameras(ShakeTransformEventData shake) {
            /*foreach (var cam in cameras) {
                cam.AddShakeEvent(shake);
            }*/
        }

        public void FinishRace(int playerID)
        {
            gameState = GameState.finish;
            var board = Instantiate(ScoreBoardPrefab); // auto id to link with kart with the same ID
            board.setId(playerID);
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

        public void ResumeGame()
        {
            
            Time.timeScale = 1;
            gamePaused = false;
        }
    }
}