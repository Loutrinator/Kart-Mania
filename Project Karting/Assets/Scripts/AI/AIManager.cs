using System;
using System.Collections;
using System.Collections.Generic;
using AI.UtilityAI;
using Game;
using Kart;
using Road.RoadPhysics;
using SplineEditor.Runtime;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    public static AIManager Instance { get; private set; }
    
    public PhysicsManager physicsManager;

    public KartBase kartPrefab;
    public AICamera AICamPrefab;
    public UtilityAIAsset AIAsset;
    public UtilityAIDebugger utilityAIDebugger;
    
    public Race circuit;

    public int nbIA = 1;
    
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
        
        physicsManager.Init(circuit.road.bezierSpline);
        
        StartCoroutine(AiUpdateHandler());
    }

    private void Start() {
        utilityAIDebugger.Init(InitRace());
    }

    private PlayerAI InitRace()
    {
        Transform[] spawnPoints = circuit.spawnPoints;
        PlayerAI ai = null;
        if (spawnPoints.Length >= nbIA) {
            for (int id = 0; id < nbIA; ++id) {
                
                Transform spawn = spawnPoints[id];
                KartBase kart = Instantiate(kartPrefab, spawn.position, spawn.rotation);
                kart.playerIndex = id;//TODO: A voir si c'est utile ou non
                
                //Linking to controls to the Kart
                UtilityAIController utilityAI = kart.gameObject.AddComponent<UtilityAIController>();
                utilityAI.utilityAIAsset = AIAsset;
                utilityAI.Init();
                PlayerAI playerAI = kart.gameObject.AddComponent<PlayerAI>();
                playerAI.kart = kart;
                playerAI.aiController = utilityAI;
                playerAI.aiController.kart = kart;
                if (id == 0) ai = playerAI;
                
                
                // Adding the camera of the player
                var kartCam = Instantiate(AICamPrefab, kart.transform.position, kart.transform.rotation);
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
            }
        } else {
#if UNITY_EDITOR
            Debug.LogError("Attempting to spawn " + nbIA + " but only " + spawnPoints.Length + " available.");
#endif
        }

        return ai;
    }

}
