using System.Collections.Generic;
using SplineEditor.Runtime;
using UnityEngine;

namespace Road.RoadPhysics {
    public class PhysicsManager : MonoBehaviour {
        private static PhysicsManager _instance;

        public static PhysicsManager instance => _instance;


        public BezierSpline road;
        public List<PhysicsObject> physicsObjects = new List<PhysicsObject>();

        private bool _hasRoad;

        public void Init() {
            if (_instance == null)
                _instance = this;
            _hasRoad = road != null;
        }

        private void FixedUpdate() {
            for (int i = physicsObjects.Count - 1; i >= 0; --i) {
                if (_hasRoad)
                {
                    BezierUtils.BezierPos pos = road.GetClosestBezierPos(physicsObjects[i].transform.position);
                    physicsObjects[i].UpdatePhysics(pos.LocalUp);
                }
                else
                {
                    physicsObjects[i].UpdatePhysics(Vector3.up);
                }
            }
        }

        public void AddPhysicsObject(PhysicsObject physicsObject) {
            physicsObjects.Add(physicsObject);
        }
    }
}
