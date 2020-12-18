using UnityEngine;

namespace RoadPhysics {
    public class PhysicsObject : MonoBehaviour {
        public Vector3 currentGravity;
        public Rigidbody rigidBody;

        protected void Awake() {
            PhysicsManager.instance.AddPhysicsObject(this);
            currentGravity = Physics.gravity;
        }

        public void UpdateGravity() {
            rigidBody.AddForce(currentGravity, ForceMode.Acceleration);
        }
    }
}
