using UnityEditor;
using UnityEngine;

namespace RoadPhysics {
    public class PhysicsObject : MonoBehaviour {
        public Vector3 currentGravity;
        public Rigidbody rigidBody;

        protected void Awake() {
            PhysicsManager.instance.AddPhysicsObject(this);
            currentGravity = Physics.gravity;
        }

        public void UpdateGravity(Vector3 groundNormal) {
            Vector3 gravity = -groundNormal * currentGravity.magnitude;
            rigidBody.AddForce(gravity, ForceMode.Acceleration);
        }
    }
}
