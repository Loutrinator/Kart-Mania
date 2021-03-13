using System.Collections.Generic;
using UnityEngine;

namespace RoadPhysics {
    public class PhysicsManager : MonoBehaviour {
        private static PhysicsManager _instance;

        public static PhysicsManager instance => _instance;


        public List<PhysicsObject> physicsObjects = new List<PhysicsObject>();
        
        public void Init() {
            if (_instance == null)
                _instance = this;
        }

        private void FixedUpdate() {
            for (int i = physicsObjects.Count - 1; i >= 0; --i) {
                physicsObjects[i].UpdateGravity();
            }
        }

        public void AddPhysicsObject(PhysicsObject physicsObject) {
            physicsObjects.Add(physicsObject);
        }
    }
}
