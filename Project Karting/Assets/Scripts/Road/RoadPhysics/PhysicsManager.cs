using System.Collections.Generic;
using SplineEditor.Runtime;
using UnityEngine;

namespace Road.RoadPhysics {
    public class PhysicsManager : MonoBehaviour {
        private static PhysicsManager _instance;

        public static PhysicsManager instance => _instance;


        public float drag = 0.05f;
        public BezierSpline road;
        public List<PhysicsObject> physicsObjects = new List<PhysicsObject>();

        private bool _hasRoad;

        public void Init(BezierSpline roadP) {
            if (_instance == null)
                _instance = this;
            road = roadP;
            _hasRoad = road != null;
        }

        private void FixedUpdate() {
            for (int i = physicsObjects.Count - 1; i >= 0; --i) {
                if (_hasRoad)
                {
                    physicsObjects[i].closestBezierPos = road.GetClosestBezierPos(physicsObjects[i].transform.position);
                    physicsObjects[i].UpdatePhysics(physicsObjects[i].closestBezierPos.LocalUp, drag);
                }
                else
                {
                    physicsObjects[i].UpdatePhysics(Vector3.up, drag);
                }
            }
        }

        public void AddPhysicsObject(PhysicsObject physicsObject) {
            physicsObjects.Add(physicsObject);
        }

        public void RemovePhysicsObject(PhysicsObject physicsObject) {
            physicsObjects.Remove(physicsObject);
        }
    }
}
