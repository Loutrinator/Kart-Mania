using System.Collections;
using System.Collections.Generic;
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
        public Minimap minimap;


        [Header("UI and HUD")] 
        [SerializeField] private HUDTimeTrialController timetrialHUDLeftPrefab;
        [SerializeField] private HUDTimeTrialController timetrialHUDRightPrefab;

        [SerializeField] private GameObject StartUIPrefab;

        [HideInInspector] public List<KartBase> karts = new List<KartBase>();

        private StartMsgAnimation startMessage;
        private float startTime;

        public CameraFollowPlayer cameraParentPrefab;

        public static GameManager Instance { get; private set; }

        public PhysicsManager physicsManager;
        public KartRespawner respawner;
        public ScoreBoard scoreBoard;

        [HideInInspector]
        public PauseMenu pauseMenu;

        private float _lastPauseTime;
        private const float MinPauseTime = 0.5f;

        private bool gamePaused;

        private IEnumerator Start()
        {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }

            RaceManager.Instance.gameState = GameState.idle;
            
            RaceManager.Instance.currentRace = LevelManager.instance.InitLevel();
            minimap.race = RaceManager.Instance.currentRace;
            minimap.DrawMinimap();

            if (physicsManager != null)
            {
                physicsManager.Init(RaceManager.Instance.currentRace.road.bezierSpline);
            }
        
            InitRace();

            if (respawner != null)
            {
                respawner.Init();
            }

            gamePaused = false;
            pauseMenu = FindObjectOfType<PauseMenu>();
            _lastPauseTime = Time.unscaledTime;

            yield return null;
        }

        private void Update() {
            minimap.UpdateMinimap();
        }

        public void Pause(PlayerConfiguration playerConfiguration) {
            if (Time.unscaledTime - _lastPauseTime < MinPauseTime) return;
            _lastPauseTime = Time.unscaledTime;
            
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                Time.timeScale = 0;
                pauseMenu.PauseGame(playerConfiguration);
            }
            else
            {
                pauseMenu.ResumeGame();
            }
        }
        
        public void StartRace() {
            for (int i = 0; i < RaceManager.Instance.playersInfo.Length; ++i) {
                RaceManager.Instance.playersInfo[i].currentLapStartTime = Time.time;
                RaceManager.Instance.playersInfo[i].lap = 0;
            }

            RaceManager.Instance.gameState = GameState.race;
            foreach (var kart in karts)
            {
                kart.effects.StopRewind();
            }
            
            LapManager.Instance.OnNewLap.Add(CheckEndRace);
        }

        private void CheckEndRace(int playerId)
        {
            
            if (RaceManager.Instance.playersInfo[playerId].lap > RaceManager.Instance.currentRace.laps)
            {
                FinishRace(playerId);
            }
        }

        private void InitRace()
        {
            SoundManager.Instance.PlayRaceMusic();
            HUDTimeTrialController._nbInstances = 0;

            RaceManager.Instance.currentRace.spawner.SetSpawners();

            var mode = LevelManager.instance.gameConfig.mode;
            if (mode == Game.GameMode.TimeTrial)
            {
                RaceManager.Instance.currentRace.road.transform.parent.Find("LOOTBOX").gameObject.SetActive(false);
            }

            RaceManager.Instance.gameState = GameState.start;
            int nbPlayerRacing = LevelManager.instance.gameConfig.players.Count;

            minimap.SetPosition(nbPlayerRacing);
            
            
            RaceManager.Instance.playersInfo = new PlayerRaceInfo[nbPlayerRacing];
            Transform[] spawnPoints = RaceManager.Instance.currentRace.spawnPoints;
            if (spawnPoints.Length >= nbPlayerRacing) {
                for (int id = 0; id < nbPlayerRacing; ++id) {
                    
                    //Extracting the player's configuration
                    PlayerConfiguration playerConfig = LevelManager.instance.gameConfig.players[id];
                    //Switching to Kart Inputs
                    playerConfig.Input.SwitchCurrentActionMap("Kart");
                    
                    //Spawning the kart
                    Transform spawn = spawnPoints[id];
                    KartBase kart = Instantiate(playerConfig.Kart, spawn.position, spawn.rotation);
                    kart.playerIndex = id;
                    
                    //Linking to controls to the Kart
                   
                    PlayerController playerController = kart.gameObject.AddComponent<PlayerController>();
                    playerController.kart = kart;
                    playerController.InitializePlayerConfiguration(playerConfig);

                    Rumbler rumbler = kart.gameObject.AddComponent<Rumbler>();
                    rumbler.SetPlayerInput(playerConfig.Input);
                    kart.rumbler = rumbler;
                    
                    // Adding the camera of the player
                    var kartCam = Instantiate(cameraParentPrefab, kart.transform.position, kart.transform.rotation);
                    if (id != 0)
                    {
                        Destroy(kartCam.GetComponent<AudioListener>());
                    }
                    kartCam.SetViewport(id);
                    kartCam.target = kart;
                    kart.cameraFollowPlayer = kartCam;
                    
                    //setting the camera to the KartEffect of the kart
                    KartEffects kartEffects = kart.GetComponent<KartEffects>();
                    if (kartEffects != null)
                    {
                        kartEffects.cameraFollowPlayer = kartCam;
                        kartEffects.cam = kartCam.cam;
                    }
                    
                    //Setting the camera to the KartAudio of the kart
                    KartAudio kartAudio = kart.GetComponent<KartAudio>();
                    if (kartAudio != null)
                    {
                        kartAudio.cam = kartCam.cam;
                    }

                    //Saving the kart in karts
                    karts.Add(kart);
                    
                    PlayerRaceInfo info = new PlayerRaceInfo(kart, id); //TODO : if human PlayerAction, if IA ComputerAction
                    info.camera = kartCam;
                    info.currentCheckpoint = RaceManager.Instance.currentRace.checkpointAmount - 1;
                    RaceManager.Instance.playersInfo[id] = info;
                    
                    //Flipping the UI if required
                    HUDTimeTrialController HUDPRefab = timetrialHUDLeftPrefab;

                    if ((nbPlayerRacing == 2 && id == 1) || (nbPlayerRacing == 3 && id == 2) ||
                        (nbPlayerRacing == 4 && id >= 2))
                    {
                        HUDPRefab = timetrialHUDRightPrefab;
                    }
                    
                    //Adding a the HUD and linking it to the cam
                    HUDTimeTrialController timetrialHUD = Instantiate(HUDPRefab); // id automatically set inside the class
                    kart.itemWheel = timetrialHUD.itemWheel;
                    Canvas timeTrialCanvas = timetrialHUD.GetComponent<Canvas>();
                    timeTrialCanvas.worldCamera = kartCam.cam;
                    timeTrialCanvas.planeDistance = 1;
                    timeTrialCanvas.sortingOrder = 100000;
                    
                    //Adding the kart marker to the minimap
                    minimap.AddVisualObject(kart.gameObject, kart.minimapRenderer, playerConfig.Color);

                }
                    
                //Adding the start countdown HUD    
                startMessage = Instantiate(StartUIPrefab).GetComponentInChildren<StartMsgAnimation>();
                    
                startTime = startMessage.getStartTime();
                StartMsgAnimation startUI = startMessage.GetComponent<StartMsgAnimation>();
                if (startUI != null)
                {
                    startUI.placeKeysAction = placeKeys;
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
        public PlayerRaceInfo GetPlayerRaceInfo(int id)
        {
            return RaceManager.Instance.GetPlayerRaceInfo(id);
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


        public void ShakeCameras(ShakeTransformEventData shake) {
            /*foreach (var cam in cameras) {
                cam.AddShakeEvent(shake);
            }*/
        }

        public void FinishRace(int playerID) {
            RaceManager.Instance.gameState = GameState.finish;
            karts[playerID].canMove = false;
            karts[playerID].effects.driftLevel = 0;
            karts[playerID].effects.StopDrift();
            karts[playerID].currentAngularVelocity = Vector3.zero;
            karts[playerID].ResetKart();
            //var board = Instantiate(ScoreBoardPrefab); // auto id to link with kart with the same ID
            scoreBoard.gameObject.SetActive(true);
            scoreBoard.SetId(playerID);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
            gamePaused = false;
        }
    }
}