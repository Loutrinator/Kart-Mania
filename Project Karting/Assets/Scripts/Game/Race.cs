using Road.RoadMesh;
using SplineEditor.Runtime;
using UnityEngine;

namespace Game {
    public class Race : MonoBehaviour
    {
        public string circuitName;
        public Sprite image;
        public int laps;

        public BezierPath road;
        
        public Transform[] spawnPoints;
        public SpawnPoints spawner;

        [HideInInspector] public int checkpointAmount;

        public void Init() {
            road.bezierMeshExtrusion.UpdateMesh();
            var borders = GetComponentsInChildren<RoadBorder>();
            foreach (var border in borders) {
                border.UpdateMesh();
            }

            checkpointAmount = GetComponentsInChildren<Checkpoint>().Length;
        }
    }
}