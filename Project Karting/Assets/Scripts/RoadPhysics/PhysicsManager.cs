using System.Collections.Generic;
using SplineEditor.Runtime;
using UnityEngine;

namespace RoadPhysics {
    public class PhysicsManager : MonoBehaviour {
        private static PhysicsManager _instance;

        public static PhysicsManager instance => _instance;


        public BezierSpline road;
        public List<PhysicsObject> physicsObjects = new List<PhysicsObject>();

        public void Init() {
            if (_instance == null)
                _instance = this;
        }

        private void FixedUpdate() {
            for (int i = physicsObjects.Count - 1; i >= 0; --i) {
                //BezierUtils.BezierPos pos = road.GetClosestPoint(physicsObjects[i].transform.position);
                physicsObjects[i].UpdateGravity(Vector3.up); //pos.LocalUp);
            }
        }

        public void AddPhysicsObject(PhysicsObject physicsObject) {
            physicsObjects.Add(physicsObject);
        }
    }
}
