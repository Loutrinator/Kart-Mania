using UnityEngine;

namespace Road.RoadPhysics {
    public abstract class PhysicsObject : MonoBehaviour {
        public Vector3 currentGravityAcceleration;
        public Vector3 currentVelocity;
        public Vector3 currentAngularVelocity;
        public Rigidbody rigidBody;

        private Vector3 _currentGravityVelocity;

        protected virtual void Awake() {
            PhysicsManager.instance.AddPhysicsObject(this);
            currentGravityAcceleration = Physics.gravity;
        }

        public void UpdatePhysics(Vector3 groundNormal) {
            currentGravityAcceleration = -groundNormal * currentGravityAcceleration.magnitude;

            if (IsGrounded()) _currentGravityVelocity = Time.fixedDeltaTime * currentGravityAcceleration;
            else
            {
                _currentGravityVelocity += Time.fixedDeltaTime * currentGravityAcceleration;
            }
            
            rigidBody.velocity = currentVelocity + _currentGravityVelocity;
            rigidBody.angularVelocity = currentAngularVelocity;
        }

        protected abstract bool IsGrounded();
    }
}
