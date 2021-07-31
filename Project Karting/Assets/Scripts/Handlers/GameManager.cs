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

        [Header("UI and HUD")] 
        [SerializeField] private HUDTimeTrialController timetrialHUDLeftPrefab;
        [SerializeField] private HUDTimeTrialController timetrialHUDRightPrefab;

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
        public ScoreBoard scoreBoard;

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
            
            if (gameState == GameState.race) {
                PlayerRaceInfo player = playersInfo[0];
                //currentTime.text = floatToTimeString(Time.time - player.currentLapStartTime);
                //lap.text = player.lap.ToString();
                //checkpoint.text = player.currentCheckpoint.ToString();
                //float diff = Time.time - player.currentLapStartTime;
                // info = "Time : " + floatToTimeString(Time.time) + "\nLap start time : " +
                //              floatToTimeString(player.currentLapStartTime) + "\nDiff : " + floatToTimeString(diff);
                //timeInfo.text = info;
                if (player.controller != null)
                {
                    //player.controller.active = true; // listen player inputs 
                }
            }
            minimap.UpdateMinimap();
        }

        public void Pause()
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
            SoundManager.Instance.PlayRaceMusic();
            HUDTimeTrialController._nbInstances = 0;
            
            gameState = GameState.start;
            int nbPlayerRacing = LevelManager.instance.gameConfig.players.Count;

            minimap.SetPosition(nbPlayerRacing);
            
            
            playersInfo = new PlayerRaceInfo[nbPlayerRacing];
            Transform[] spawnPoints = currentRace.spawnPoints;
            if (spawnPoints.Length >= nbPlayerRacing) {
                for (int id = 0; id < nbPlayerRacing; ++id) {
                    
                    //Extracting the player's configuration
                    PlayerConfiguration playerConfig = LevelManager.instance.gameConfig.players[id];
                    //Switching to Kart Inputs
                    playerConfig.Input.SwitchCurrentActionMap("Kart");
                    
                    //Spawning the kart
                    Transform spawn = spawnPoints[id];
                    KartBase kart = Instantiate(playerConfig.KartPrefab, spawn.position, spawn.rotation);
                    kart.playerIndex = id;
                    
                    //Linking to controls to the Kart
                    PlayerController playerController = kart.GetComponent<PlayerController>();
                    playerController.InitializePlayerConfiguration(playerConfig);
                    
                    
                    // Adding the camera of the player
                    var kartCam = Instantiate(cameraParentPrefab, kart.transform.position, kart.transform.rotation);
                    if (id != 0)
                    {
                        Destroy(kartCam.GetComponent<AudioListener>());
                    }
                    kartCam.SetViewport(id);
                    kartCam.target = kart.transform;
                    
                    //setting the camera to the KartEffect of the kart
                    KartEffects kartEffects = kart.GetComponent<KartEffects>();
                    if (kartEffects != null)
                    {
                        kartEffects.cameraShakeTransform = kartCam.cameraShakeTransform;
                        kartEffects.cam = kartCam.cam;
                    }
                    
                    //Setting the camera to the KartAudio of the kart
                    KartAudio kartAudio = kart.GetComponent<KartAudio>();
                    if (kartAudio != null)
                    {
                        kartAudio.cam = kartCam.cam;
                    }

                    kart.cameraFollowPlayer = kartCam;
                    //Saving the kart in karts
                    karts.Add(kart);
                    
                    PlayerRaceInfo info = new PlayerRaceInfo(kart, id); //TODO : if human PlayerAction, if IA ComputerAction
                    info.camera = kartCam;
                    playersInfo[id] = info;
                    
                    //Flipping the UI if required
                    HUDTimeTrialController HUDPRefab = timetrialHUDLeftPrefab;

                    if ((nbPlayerRacing == 2 && id == 1) || (nbPlayerRacing == 3 && id == 2) ||
                        (nbPlayerRacing == 4 && id >= 2))
                    {
                        HUDPRefab = timetrialHUDRightPrefab;
                    }
                    
                    //Adding a the HUD and linking it to the cam
                    Canvas timetrialHUD = Instantiate(HUDPRefab).GetComponent<Canvas>(); // id automatically set inside the class
                    timetrialHUD.worldCamera = kartCam.cam;
                    timetrialHUD.planeDistance = 1;
                    timetrialHUD.sortingOrder = 100000;
                    
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
                
                
                gameState = GameState.start;
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
            if (checkpointId < currentRace.checkpointAmount) {
                if (checkpointId - player.currentCheckpoint == 1) {
                    playersInfo[playerId].currentCheckpoint = checkpointId;
                }
                else if (player.currentCheckpoint == currentRace.checkpointAmount - 1 && checkpointId == 0) {
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
                Debug.Log("time add " + Utils.DisplayHelper.FloatToTimeString(t));   
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

            if (playersInfo[playerId].lap > currentRace.laps)
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

        public void FinishRace(int playerID) {
            gameState = GameState.finish;
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