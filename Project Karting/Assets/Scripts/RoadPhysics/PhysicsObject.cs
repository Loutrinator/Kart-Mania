using UnityEngine;

namespace RoadPhysics {
    public class PhysicsObject : MonoBehaviour {
        public Vector3 currentGravity;
        public Rigidbody rigidBody;

        protected virtual void Awake() {
            PhysicsManager.instance.AddPhysicsObject(this);
            currentGravity = Physics.gravity;
        }

        public void UpdateGravity(Vector3 groundNormal) {
            currentGravity = -groundNormal * currentGravity.magnitude;
            rigidBody.AddForce(currentGravity, ForceMode.Acceleration);
        }
    }
}
