using SplineEditor.Runtime;
using UnityEngine;

namespace Road.RoadPhysics {
    public abstract class PhysicsObject : MonoBehaviour {
        public Vector3 currentGravityAcceleration;
        public Vector3 currentVelocity;
        public Vector3 currentAngularVelocity;
        public Rigidbody rigidBody;

        public BezierUtils.BezierPos closestBezierPos;
        public BezierUtils.BezierPos lastGroundBezierPos;
        private Vector3 _currentGravityVelocity;

        protected virtual void Awake() {
            PhysicsManager.instance.AddPhysicsObject(this);
            currentGravityAcceleration = Physics.gravity;
        }

        public void UpdatePhysics(Vector3 groundNormal) {
            currentGravityAcceleration = -groundNormal * currentGravityAcceleration.magnitude;

            if (IsGrounded())
            {
                _currentGravityVelocity = Time.fixedDeltaTime * currentGravityAcceleration;
                lastGroundBezierPos = closestBezierPos;
            }
            else
            {
                _currentGravityVelocity += Time.fixedDeltaTime * currentGravityAcceleration;
            }
            
            rigidBody.velocity = currentVelocity + _currentGravityVelocity;
            rigidBody.angularVelocity = currentAngularVelocity;
        }

        public abstract bool IsGrounded();
    }
}
