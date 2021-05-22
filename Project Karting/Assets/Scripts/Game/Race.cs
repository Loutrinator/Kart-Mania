using SplineEditor.Runtime;
using UnityEngine;

namespace Game {
    public class Race : MonoBehaviour
    {
        public string circuitName;
        public Sprite image;
        public int laps;

        [SerializeField] private BezierPath road;
        
        public Transform[] spawnPoints;

        public void Init() {
            road.bezierMeshExtrusion.UpdateMesh();
        }
    }
}