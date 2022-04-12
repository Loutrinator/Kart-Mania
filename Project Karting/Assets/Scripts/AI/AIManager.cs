using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.UtilityAI;
using Game;
using Genetics;
using Kart;
using Road.RoadPhysics;
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

            StartCoroutine(AiUpdateHandler());
        }

        private void Start() {
            if (nbIA > circuit.spawnPoints.Length) nbIA = circuit.spawnPoints.Length;

            var genomes = new List<AIGenome>();
            var genomeFromFile = GeneticsUtils.GetDataFromFile(genomeFileName);
            for (int i = 0; i < nbIA; ++i) {
                genomes.Add(genomeFromFile.RandomizeGenome());
            }

            var karts = InitRace(genomes);
            utilityAIDebugger.Init(karts[0]);
        }

        private PlayerAI[] InitRace(List<AIGenome> genomes) {
            Transform[] spawnPoints = circuit.spawnPoints;
            PlayerAI[] ais = new PlayerAI[nbIA];
            if (spawnPoints.Length >= nbIA) {
                AICamera kartCam = null;
                for (int id = 0; id < nbIA; ++id) {
                    Transform spawn = spawnPoints[id];
                    KartBase kart = Instantiate(kartPrefab, spawn.position, spawn.rotation);

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
                    if (id == 0) {
                        // Adding the camera of the player
                        kartCam = Instantiate(AICamPrefab, kart.transform.position, kart.transform.rotation);
                        kartCam.target = kart.transform;
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
                }
            }
            else {
#if UNITY_EDITOR
                Debug.LogError("Attempting to spawn " + nbIA + " but only " + spawnPoints.Length + " available.");
#endif
            }

            _clockStart = Time.time;

            return ais;
        }

        private AIGenome _bestGenome;
        private void OnRaceEnd() {
            _updateAi = false;
            StopCoroutine(AiUpdateHandler());
            
            var karts = FindObjectsOfType<UtilityAIController>();
            var sortedKarts = RaceUtils.GetRankingUtilityAI(karts);

            _bestGenome = sortedKarts[0].genome;
            
            var children = GeneticsUtils.GenerateChildren(sortedKarts.Select(k => k.genome).ToList(), nbIA, 4);
            
            // clear karts
            for (int i = karts.Length - 1; i >= 0; --i) {
                Destroy(karts[i]);
            }
            playersAiUpdate.Clear();
            StartCoroutine(AiUpdateHandler());
            
            var newKarts = InitRace(children);
            utilityAIDebugger.Init(newKarts[0]);

            _updateAi = true;

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            AssetDatabase.Refresh();
#endif
        }

        private void OnApplicationQuit() {
            GeneticsUtils.WriteData(_bestGenome, "TrainedData.json");
        }
    }
}