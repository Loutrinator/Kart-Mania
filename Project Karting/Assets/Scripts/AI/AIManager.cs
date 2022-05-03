using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.UtilityAI;
using Game;
using Genetics;
using Handlers;
using Kart;
using Road.RoadPhysics;
using SplineEditor.Runtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using AIGenome =
    System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<float>>>;

namespace AI {
    public enum RaceMode {
        Time,
        Laps
    }

    public class AIManager : MonoBehaviour {
        [SerializeField] private string genomeFileName;
        [SerializeField] private float clockTimeSeconds = 20f;
        [SerializeField] private TextMeshProUGUI clockText;
        public static AIManager Instance { get; private set; }

        public PhysicsManager physicsManager;

        public KartBase kartPrefab;
        public AICamera AICamPrefab;
        public UtilityAIAsset AIAsset;
        public UtilityAIDebugger utilityAIDebugger;

        public Race circuit;

        public int nbIA = 1;

        private float _clockStart;

        #region IA_HANDLER

        public List<Action> playersAiUpdate = new List<Action>();
        private float _aiFrameLength = 0.1f; // time to wait between 2 ai update frames
        private bool _updateAi; // true when game start, false on game stop

        private List<KartBase> racingKarts;
        private List<AICamera> racingCams;
        private float bestPreviousDistance = 0;

        private int currentNbItterations = 1;
        
        private IEnumerator AiUpdateHandler() {
            var wait = new WaitForSeconds(_aiFrameLength);
            while (_updateAi) {
                for (int i = playersAiUpdate.Count - 1; i >= 0; --i) {
                    playersAiUpdate[i].Invoke();
                }

                clockText.text = (Time.time - _clockStart).ToString();
                if (Time.time - _clockStart > clockTimeSeconds) {
                    _updateAi = false;
                }

                yield return wait;
            }

            OnRaceEnd();
        }

        #endregion

        void Awake() {
            if (Instance == null) {
                Instance = this;
                //cameras = new List<ShakeTransform>();
            }
            else {
                Destroy(gameObject);
            }

            _updateAi = true;

            physicsManager.Init(circuit.road.bezierSpline);
            //RaceManager.Instance.currentRace = circuit;
            
            StartCoroutine(AiUpdateHandler());
        }

        private void Start() {

            var genomes = new List<AIGenome>();
            var genomeFromFile = GeneticsUtils.GetDataFromFile(genomeFileName);
            for (int i = 0; i < nbIA; ++i) {
                genomes.Add(genomeFromFile.RandomizeGenome());
            }

            racingKarts = new List<KartBase>();
            racingCams = new List<AICamera>();
            var karts = InitRace(genomes);
            utilityAIDebugger.Init(karts[0]);
        }

        private void Update()
        {
            //Finds the longest distance traveled by a kart
            float furthestKartDist = -1;
            float circuitLength = circuit.road.bezierSpline.bezierLength;
            foreach (var playerInfo in RaceManager.Instance.playersInfo)
            {
                
                if (playerInfo.kart.closestBezierPos != null)
                {
                    float dist = playerInfo.getDistanceTraveled(circuitLength);
                    if (dist > furthestKartDist)
                    {
                        furthestKartDist = dist;
                    }
                }
            }
            utilityAIDebugger.SetDistances(bestPreviousDistance, furthestKartDist);

        }

        private PlayerAI[] InitRace(List<AIGenome> genomes)
        {

            circuit.Init();
            RaceManager.Instance.playersInfo = new PlayerRaceInfo[nbIA];
            
            utilityAIDebugger.SetItterations(currentNbItterations);
            
            Transform[] spawnPoints = circuit.spawnPoints;
            PlayerAI[] ais = new PlayerAI[nbIA];

            RemoveKarts();

            AICamera kartCam = null;
            
            for (int id = 0; id < nbIA; ++id) {
                Transform spawn = spawnPoints[id%spawnPoints.Length];
                KartBase kart = Instantiate(kartPrefab, spawn.position, spawn.rotation);
                racingKarts.Add(kart);
                var colliders = kart.GetComponentsInChildren<Collider>();
                foreach (var col in colliders) {
                    col.gameObject.layer = LayerMask.NameToLayer("KartsIA");
                }
                
                kart.playerIndex = id; //TODO: A voir si c'est utile ou non

                //Linking to controls to the Kart
                UtilityAIController utilityAI = kart.gameObject.AddComponent<UtilityAIController>();
                utilityAI.utilityAIAsset = AIAsset;

                utilityAI.Init(genomes[id]);
                Debug.Log(genomes[id].GetString());

                PlayerAI playerAI = kart.gameObject.AddComponent<PlayerAI>();
                playerAI.kart = kart;
                playerAI.aiController = utilityAI;
                playerAI.aiController.kart = kart;

                ais[id] = playerAI;


                KartEffects kartEffects = kart.GetComponent<KartEffects>();
                // Adding the camera of the player
                if (id == 0)
                {
                    kartCam = Instantiate(AICamPrefab, kart.transform.position, kart.transform.rotation);
                    kartCam.target = kart;
                    racingCams.Add(kartCam);
                }

                //setting the camera to the KartEffect of the kart
                if (kartEffects != null) {
                    kartEffects.cameraShakeTransform = kartCam.cameraShakeTransform;
                    kartEffects.cam = kartCam.cam;
                }

                //Setting the camera to the KartAudio of the kart
                KartAudio kartAudio = kart.GetComponent<KartAudio>();
                if (kartAudio != null) {
                    kartAudio.cam = kartCam.cam;
                }
                
                //PlayerInfo
                PlayerRaceInfo info = new PlayerRaceInfo(kart, id); //TODO : if human PlayerAction, if IA ComputerAction
                info.currentLapStartTime = Time.time;
                info.lap = 0;
                info.camera = kartCam;
                info.currentCheckpoint = RaceManager.Instance.currentRace.checkpointAmount - 1;

                //getting the kart position on the circuit when instanciated
                kart.closestBezierPos = circuit.road.bezierSpline.GetClosestBezierPos(kart.transform.position);
                info.spawnDistance = kart.closestBezierPos.BezierDistance;
                
                RaceManager.Instance.playersInfo[id] = info;
            }

            _clockStart = Time.time;

            return ais;
        }

        private void RemoveKarts()
        {
            foreach (var kart in racingKarts)
            {
                Destroy(kart.gameObject);
            }

            foreach (var cam in racingCams)
            {
                Destroy(cam.gameObject);
            }

            racingKarts = new List<KartBase>();
            racingCams = new List<AICamera>();
        }

        private AIGenome _bestGenome;
        private void OnRaceEnd() {
            _updateAi = false;
            //StopCoroutine(AiUpdateHandler());
            StopAllCoroutines();
            
            var sortedKarts = RaceUtils.GetRankingUtilityAI(RaceManager.Instance.playersInfo, circuit.road.bezierSpline.bezierLength);
            bestPreviousDistance = sortedKarts[0].getDistanceTraveled(circuit.road.bezierSpline.bezierLength);
            UtilityAIController aiController = sortedKarts[0].kart.GetComponent<UtilityAIController>();
            _bestGenome = aiController.genome;
            
            var children = GeneticsUtils.GenerateChildren(sortedKarts.Select(k => k.kart.GetComponent<UtilityAIController>().genome).ToList(), nbIA, 4);
            
            // clear karts
            
            playersAiUpdate.Clear();
            RemoveKarts();

            currentNbItterations++;
            
            //_clockStart = Time.time;
            var newKarts = InitRace(children);
            utilityAIDebugger.Init(newKarts[0]);
            
            _updateAi = true;
            StartCoroutine(AiUpdateHandler());
            
            

        }

        private void OnApplicationQuit() {

            //GeneticsUtils.WriteData(_bestGenome, "TrainedData.json");
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}