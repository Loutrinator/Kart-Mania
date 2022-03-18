using System;
using System.Collections;
using System.Collections.Generic;
using Road.RoadPhysics;
using SplineEditor.Runtime;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    public static AIManager Instance { get; private set; }
    
    public PhysicsManager physicsManager;

    public BezierPath circuit;
    
    #region IA_HANDLER
    
    public List<Action> playersAiUpdate = new List<Action>();
    private float _aiFrameLength = 0.1f;    // time to wait between 2 ai update frames
    private bool _updateAi; // true when game start, false on game stop
    // todo start this coroutine
    private IEnumerator AiUpdateHandler() {
        var wait = new WaitForSeconds(_aiFrameLength);
        while (_updateAi) {
            for (int i = playersAiUpdate.Count - 1; i >= 0; --i) {
                playersAiUpdate[i].Invoke();
            }

            yield return wait;
        }
    }
    #endregion

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            //cameras = new List<ShakeTransform>();
        }
        else {
            Destroy(gameObject);
        }

        _updateAi = true;
        StartCoroutine(AiUpdateHandler());
    }
    void Start()
    {
        StartCoroutine(StartCoroutine());
    }

    private IEnumerator StartCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        physicsManager.Init(circuit.bezierSpline);
    }
    
    /*
        private void InitRace()
        {
            
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
*/

}
