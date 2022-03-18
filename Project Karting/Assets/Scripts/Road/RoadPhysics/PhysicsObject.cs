using System;
using SplineEditor.Runtime;
using UnityEngine;

namespace Road.RoadPhysics {
    public abstract class PhysicsObject : MonoBehaviour {
        public Vector3 currentGravityAcceleration;
        public Vector3 currentVelocity;
        public Vector3 currentAngularVelocity;
        public Vector3 currentForcesVelocity;
        public Rigidbody rigidBody;

        public BezierUtils.BezierPos closestBezierPos;
        public BezierUtils.BezierPos lastGroundBezierPos;
        private Vector3 _currentGravityVelocity;

        protected virtual void Awake() {
            PhysicsManager.instance.AddPhysicsObject(this);
            currentGravityAcceleration = Physics.gravity;
        }

        public void UpdatePhysics(Vector3 groundNormal, float drag)
        {
            currentGravityAcceleration = -groundNormal * currentGravityAcceleration.magnitude;

            if (IsGrounded())
            {
                _currentGravityVelocity = currentGravityAcceleration * Time.fixedDeltaTime;
                lastGroundBezierPos = closestBezierPos;
            }
            else
            {
                _currentGravityVelocity += currentGravityAcceleration * (Time.fixedDeltaTime * KartPhysicsSettings.instance.gravityMultiplier);
            }
            
            /*rigidBody.velocity = currentVelocity + _currentGravityVelocity + currentForcesVelocity;
            rigidBody.angularVelocity = currentAngularVelocity;*/
            
            // drag
            currentForcesVelocity -= currentForcesVelocity * drag;
            
            rigidBody.AddForce(currentGravityAcceleration * KartPhysicsSettings.instance.gravityMultiplier, ForceMode.Acceleration);
            rigidBody.angularVelocity = currentAngularVelocity;
        }

        public abstract bool IsGrounded();

        public void ResetForces() {
            currentForcesVelocity = Vector3.zero;
            currentVelocity = Vector3.zero;
            _currentGravityVelocity = Vector3.zero;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(rigidBody.position, currentGravityAcceleration);
        }
    }
}
