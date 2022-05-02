using System;
using System.Collections.Generic;
using Handlers;
using Road.RoadPhysics;
using UnityEngine;

namespace Kart
{
    public class KartBase : PhysicsObject
    {
        public int playerIndex;
        
        public GameObject minimapRenderer;
        public Transform kartRootModel; // The kart's root 3D model
        public Transform kartBodyModel; // The main kart 3D model (no wheels)
        public List<WheelCollider> wheels; //TODO: A DEPLACER
        public List<Transform> turningWheels; // The turning wheels of the kart

        //public Transform rotationAxis;
        public KartEffects effects;
        public Keyhole keyhole;

        public Stats vehicleStats = new Stats
        {
            topSpeed = .5f,
            acceleration = .5f,
            braking = .5f,
            reverseAcceleration = .5f,
            reverseSpeed = .5f,
            steer = .5f,
            addedGravity = .5f,
            suspension = .5f
        };
        
        List<StatPowerup> activePowerupList = new List<StatPowerup>();
        private Stats convertedStats;
        private Stats finalStats;

        protected float lerpedAngle;
        [HideInInspector] public Vector2 movement; // -1; 0; 1
        [HideInInspector] public bool drift;
        [HideInInspector] public int driftDirection;
        [HideInInspector] public bool rear;

        private bool drifting;

        // PlayerRaceInfo (who's listening is own kart GetPlayerID) will return the associated player ID
        public Func<int> GetPlayerID;

        private Vector3 _firstPos;
        private float _firstPosTime;
        private float _currentSpeed;
        private float _yVelocity;
        private float _currentAngularSpeed;
        private float _lerpedWheelDirection;
        private float _lerpedKartRotation;

        public bool canMove;
        

        protected void Awake()
        {
            if(transform == null) InitTransform();
            _firstPos = transform.position;
            _firstPosTime = Time.time;
            StopDrifting();
            canMove = true;

            var colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                foreach (var wheel in wheels)
                {
                    if(wheel != col)
                        Physics.IgnoreCollision(wheel, col);
                }
            }
        }

        private RaycastHit[] _overlapResults = new RaycastHit[1];
        public override bool IsGrounded()
        {
            int wheelsOnGround = 0;
            var layerMaskEnv = LayerMask.GetMask("Environment");
            var layerMaskGround = LayerMask.GetMask("Ground");
            
            for (var index = 0; index < wheels.Count; index++) {
                Vector3 wheelPos = wheels[index].transform.position;
                //if (wheels[index].isGrounded) wheelsOnGround++;
                if (Physics.SphereCastNonAlloc(wheelPos, 0.1f, -transform.up, _overlapResults, 0.5f, layerMaskEnv) > 0)
                {
                    GameManager.Instance.respawner.Respawn(this);
                }
                else if (Physics.SphereCastNonAlloc(wheelPos, 0.1f, -transform.up, _overlapResults, 0.5f, layerMaskGround) > 0) {
                    wheelsOnGround++;
                }
            }

            return wheelsOnGround >= 1;
        }

        private void FixedUpdate()
        {

            
            //if (GameManager.Instance.gameState == GameState.start)
            //{
            /*
                if (rear)
                {
                    cameraFollowPlayer.switchCameraMode(CameraMode.rear);
                }
                else
                {
                    cameraFollowPlayer.switchCameraMode(CameraMode.front);
                }

                if (movement[1] > 0)
                {
                    effects.Rewind();
                }*/
            //}
            //if (!GameManager.Instance.RaceHadBegun() || !canMove) return;
            ConvertStats();
            ApplyPowerups();

            float rotationDirection = movement[1] > 0 ? movement[0] : -movement[0];
            
            
            if (IsGrounded())
            {
                Move(movement[1]);
                if (IsGrounded() && drift && !drifting && (movement[0] < 0 || movement[0] > 0))
                {
                    StartDrift(rotationDirection);
                }
            }
            else
            {
                if (movement[1] != 0)
                {
                    rotationDirection = 0;
                    currentAngularVelocity = Vector3.zero;
                }

            }

            if (movement[1] != 0) //on tourne pas à l'arret
            {
                if (drifting) {
                    if (!drift) {
                        StopDrifting();
                    }else {
                        float driftAngle = (1 + rotationDirection * driftDirection) / 2 * (KartPhysicsSettings.instance.maxDriftAngle - KartPhysicsSettings.instance.minDriftAngle) +
                                           KartPhysicsSettings.instance.minDriftAngle;
                        driftAngle *= driftDirection;
                        Rotate(driftAngle);
                    }
                }else {
                    Rotate(rotationDirection);
                }
            }else {
                currentAngularVelocity = Vector3.zero;
            }
            
            rigidBody.AddForce(transform.forward * (_currentSpeed), ForceMode.Acceleration);
        }

        private void ConvertStats()
        {
            convertedStats.acceleration = vehicleStats.acceleration * KartPhysicsSettings.instance.acceleration;
            convertedStats.topSpeed = KartPhysicsSettings.instance.getTopSpeed(vehicleStats.topSpeed);
            convertedStats.reverseAcceleration = vehicleStats.reverseAcceleration * KartPhysicsSettings.instance.reverseAcceleration;
            convertedStats.reverseSpeed = vehicleStats.reverseSpeed * KartPhysicsSettings.instance.reverseSpeed;
        }


        private void Move(float direction)
        {
            if (direction > 0)
            {
                _currentSpeed += finalStats.acceleration * Time.fixedDeltaTime;
                _currentSpeed = Mathf.Min(finalStats.topSpeed, _currentSpeed);
            }
            else if (direction < 0) {
                _currentSpeed -= finalStats.reverseAcceleration* Time.fixedDeltaTime;
                _currentSpeed = Mathf.Max(-finalStats.reverseSpeed, _currentSpeed);
            }
            else _currentSpeed = Mathf.Lerp(_currentSpeed, 0, KartPhysicsSettings.instance.engineBrakeSpeed * Time.fixedDeltaTime);

            var t = transform;
            currentVelocity = t.forward * _currentSpeed;
        }

        protected void Rotate(float angle)
        {
                
            lerpedAngle = Mathf.Lerp(lerpedAngle, angle, KartPhysicsSettings.instance.kartRotationLerpSpeed * Time.fixedDeltaTime);
            float steerAngle = lerpedAngle * (finalStats.steer * 2 + KartPhysicsSettings.instance.steeringSpeed) * Time.fixedDeltaTime;

            currentAngularVelocity = transform.up * steerAngle * KartPhysicsSettings.instance.kartRotationCoeff;
            
            kartRootModel.localEulerAngles = Vector3.up * (steerAngle * KartPhysicsSettings.instance.kartModelRotationCoeff);

            kartBodyModel.localEulerAngles = Vector3.forward * (steerAngle * KartPhysicsSettings.instance.kartRollCoeff);
        }

        public void ResetKart() {
            kartRootModel.localEulerAngles = Vector3.up;
            kartBodyModel.localEulerAngles = Vector3.forward;
        }

        private void AnimateWheels()
        {
            _lerpedWheelDirection =
                Mathf.Lerp(_lerpedWheelDirection, movement[0], KartPhysicsSettings.instance.kartRotationLerpSpeed * Time.fixedDeltaTime * 2f);
            float angularSpeed = (_currentSpeed / (0.38f * (float) Math.PI) * 360f) % 360f;
            foreach (var turningWheel in turningWheels)
            {
                turningWheel.localEulerAngles = Vector3.up * (_lerpedWheelDirection * KartPhysicsSettings.instance.kartWheelAngle);
            }

            foreach (var wheel in wheels)
            {
                //wheel.localEulerAngles = Vector3.right * angularSpeed;
            }
        }

        #region Drift

        private void StartDrift(float direction)
        {
            driftDirection = direction > 0 ? 1 : direction < 0 ? -1 : 0;
            drifting = true;
            effects.StartDrift();
        }

        private void StopDrifting()
        {
            driftDirection = 0;
            _lerpedKartRotation = 0f;
            drifting = false;
            effects.StopDrift();
        }

        #endregion

        #region PowerUps

        void ApplyPowerups()
        {
            // on supprime tout powerup qui a dépassé son temps d'activation
            activePowerupList.RemoveAll((p) =>
            {
                if (p.elapsedTime > p.maxTime)
                {
                    p.powerupUsed?.Invoke();
                    return true;
                }

                return false;
            });
            var powerups = new Stats(); // on initialise des stats vierges pour nos powerups
            // on ajoute à 'powerups' les modifiers de chaque powerup
            for (int i = 0; i < activePowerupList.Count; i++)
            {
                
                var p = activePowerupList[i];
                // on met a jour le compteur de temps écoulé depuis l'obtention du powerup
                p.elapsedTime += Time.fixedDeltaTime;
                // on additionne les modifications des stats de notre powerup à 'powerups'
                powerups += p.modifiers;
            }

            // on ajoute tous nos powerups cumulés à nos stats de base du véhicule
            finalStats = convertedStats + powerups;
            
            // on clamp toutes les valeurs des stats qui nécessitent de pas dépasser [0,1]
            finalStats.suspension = Mathf.Clamp(finalStats.suspension, 0, 1);
        }

        public void AddPowerup(StatPowerup powerup)
        {
            activePowerupList.Add(powerup);
        }

        #endregion

        public float CurrentSpeed()
        {
            return _currentSpeed;
        }
    }
}