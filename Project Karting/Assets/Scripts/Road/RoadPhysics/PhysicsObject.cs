using UnityEngine;

namespace RoadPhysics {
    public abstract class PhysicsObject : MonoBehaviour {
        public Vector3 currentGravityAcceleration;
        public Vector3 currentVelocity;
        public Rigidbody rigidBody;

        private Vector3 currentGravityVelocity;

        protected virtual void Awake() {
            PhysicsManager.instance.AddPhysicsObject(this);
            currentGravityAcceleration = Physics.gravity;
        }

        public void UpdatePhysics(Vector3 groundNormal) {
            currentGravityAcceleration = -groundNormal * currentGravityAcceleration.magnitude;

            if (IsGrounded()) currentGravityVelocity = Time.fixedDeltaTime * currentGravityAcceleration;
            else
            {
                currentGravityVelocity += Time.fixedDeltaTime * currentGravityAcceleration;
            }
            
            rigidBody.velocity = currentVelocity + currentGravityVelocity;
            rigidBody.angularVelocity = Vector3.zero;
            //rigidBody.AddForce(currentGravityAcceleration, ForceMode.Acceleration);
        }

        protected abstract bool IsGrounded();
    }
}
