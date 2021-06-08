using Game;
using Items;
using Kart;
using Player;
using Road.RoadPhysics;
using UnityEngine;

namespace Handlers {
    public class GameManager : MonoBehaviour {
        public Race currentRace;

        public ItemManager itemManager;
        public int checkpointAmount;

        /*[Header("UI and HUD")] [SerializeField]
        private GameObject HUDvsClockPrefab;

        [SerializeField] private GameObject StartUIPrefab;*/

        private PlayerRaceInfo[] playersInfo;

        private bool raceBegan;
        private bool raceIsInit;
        private StartMsgAnimation startMessage;

        //private List<ShakeTransform> cameras;
        public GameObject cameraParentPrefab;

        public static GameManager Instance { get; private set; }

        public PhysicsManager physicsManager;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
                //cameras = new List<ShakeTransform>();
            }
            else {
                Destroy(gameObject);
            }

            raceIsInit = false;
            
            Race currentRace = LevelManager.instance.InitLevel();
            
            if (physicsManager != null)
            {
                physicsManager.Init(currentRace.road.bezierSpline);
            }
            
            InitRace();
            
            TransitionController.Instance.FadeOut(() => {
                raceBegan = true;  // todo enable after delay
            });
        }

        private void Update() {
            if (raceBegan) {
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
        }

        public void StartRace() {
            for (int i = 0; i < playersInfo.Length; ++i) {
                playersInfo[i].currentLapStartTime = Time.time;
                playersInfo[i].lap = 1;
            }

            raceBegan = true;
        }


        private void InitRace() {
            int nbPlayerRacing = LevelManager.instance.gameConfig.players.Count;
            playersInfo = new PlayerRaceInfo[nbPlayerRacing];
            Transform[] spawnPoints = LevelManager.instance.currentRace.spawnPoints;
            if (spawnPoints.Length >= nbPlayerRacing) {
                for (int id = 0; id < nbPlayerRacing; ++id) {
                    PlayerConfig playerConfig = LevelManager.instance.gameConfig.players[id];
                    Transform spawn = spawnPoints[id];
                    KartBase kart = Instantiate(playerConfig.kartPrefab, spawn.position, spawn.rotation);
                    
                    PlayerRaceInfo info = new PlayerRaceInfo(kart, id, new PlayerAction()); //TODO : if human PlayerAction, if IA ComputerAction
                    playersInfo[id] = info;
                    
                    // cameras for players
                    Instantiate(cameraParentPrefab, kart.transform.position, kart.transform.rotation);
                    
                    // todo
                    /*Instantiate(HUDvsClockPrefab); // id automatically set inside the class
                    startMessage = Instantiate(StartUIPrefab).GetComponentInChildren<StartMsgAnimation>();
                    ShakeTransform cam = kart.cameraShake;
                    if (cam != null) {
                        cameras.Add(cam);
                    }
                    startMessage._startTime = Time.time;*/
                    raceIsInit = true;
                }
            } else {
#if UNITY_EDITOR
                Debug.LogError("Attempting to spawn " + nbPlayerRacing + " but only " + spawnPoints.Length + " available.");
#endif
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

        public bool RaceHasBegan() {
            return raceBegan;
        }

        public void ShakeCameras(ShakeTransformEventData shake) {
            /*foreach (var cam in cameras) {
                cam.AddShakeEvent(shake);
            }*/
        }
    }
}