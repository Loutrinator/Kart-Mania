using System;
using System.Collections.Generic;
using Handlers;
using Road.RoadPhysics;
using UnityEngine;

namespace Kart
{
    public class KartBase : PhysicsObject
    {
        public GameObject minimapRenderer;
        public Transform kartRootModel; // The kart's root 3D model
        public Transform kartBodyModel; // The main kart 3D model (no wheels)
        public List<Transform> turningWheels; // The turning wheels of the kart

        public ShakeTransform cameraShake;

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

        [HideInInspector] public float hMove;
        protected float lerpedAngle;
        [HideInInspector] public int forwardMove; // -1; 0; 1
        [HideInInspector] public bool drift;
        [HideInInspector] public int driftDirection;

        private bool drifting;

        // PlayerRaceInfo (who's listening is own kart GetPlayerID) will return the associated player ID
        public Func<int> GetPlayerID;

        private float _currentSpeed;
        private float _yVelocity;
        private float _currentAngularSpeed;
        private float _lerpedWheelDirection;
        private float _lerpedKartRotation;

        public bool canMove;

        private Vector3 _kartBallOffset;

        protected override void Awake()
        {
            base.Awake();
            StopDrifting();
            canMove = true;

            _kartBallOffset = rigidBody.transform.localPosition;
            rigidBody.transform.parent = null;
        }

        public override bool IsGrounded() {
            return Physics.SphereCast(transform.position, 0.1f, -transform.up, out _, 0.5f, LayerMask.GetMask("Ground"));
        }

        private void FixedUpdate()
        {
            
            if (GameManager.Instance.gameState == GameState.start)
            {
                if (Input.GetAxis("Accelerate") > 0)
                {
                    effects.Rewind();
                }
            }
            if (!GameManager.Instance.RaceHadBegun() || !canMove) return;
            ConvertStats();
            ApplyPowerups();
            Move(forwardMove);
            float rotationDirection = localCurrentVelocity.z > 0 ? hMove : -hMove;

            if (drift && !drifting && (hMove < 0 || hMove > 0))
            {
                StartDrift(rotationDirection);
            }

            if (forwardMove != 0 || Mathf.Abs(localCurrentVelocity.z) > 5f) //on tourne pas à l'arret
            {
                if (drifting)
                {
                    if (!drift)
                    {
                        StopDrifting();
                    }
                    else
                    {
                        float driftAngle = (1 + rotationDirection * driftDirection) / 2 * (KartPhysicsSettings.instance.maxDriftAngle - KartPhysicsSettings.instance.minDriftAngle) +
                                           KartPhysicsSettings.instance.minDriftAngle;
                        driftAngle *= driftDirection;
                        Rotate(driftAngle);
                    }
                }
                else
                {
                    Rotate(rotationDirection);
                }
            }
            else {
                currentAngularVelocity = Vector3.zero;
            }

            _currentRotate = Mathf.Lerp(_currentRotate, _rotate, Time.deltaTime * 4f);
            _rotate = 0f;
            
            transform.position = rigidBody.transform.position - _kartBallOffset;
            Vector3 euler = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + _currentRotate, 0), Time.deltaTime * 5f);
            transform.up = closestBezierPos.LocalUp;
            transform.eulerAngles = euler;
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
            //currentVelocity = t.forward * _currentSpeed;
            
            rigidBody.AddForce(t.forward * _currentSpeed, ForceMode.Acceleration);
        }

        private float _rotate, _currentRotate;
        protected void Rotate(float angle)
        {
            _rotate = finalStats.steer * angle;
            /*lerpedAngle = Mathf.Lerp(lerpedAngle, angle, KartPhysicsSettings.instance.kartRotationLerpSpeed * Time.fixedDeltaTime);
            float steerAngle = lerpedAngle * (finalStats.steer * 2 + KartPhysicsSettings.instance.steeringSpeed) * Time.fixedDeltaTime;

            currentAngularVelocity = transform.up * steerAngle;
            
            kartRootModel.localEulerAngles = Vector3.up * (steerAngle * KartPhysicsSettings.instance.kartRotationCoeff);

            kartBodyModel.localEulerAngles = Vector3.forward * (steerAngle * KartPhysicsSettings.instance.kartRollCoeff);*/
        }

        public void ResetKart() {
            kartRootModel.localEulerAngles = Vector3.up;
            kartBodyModel.localEulerAngles = Vector3.forward;
        }

        private void AnimateWheels()
        {
            _lerpedWheelDirection =
                Mathf.Lerp(_lerpedWheelDirection, hMove, KartPhysicsSettings.instance.kartRotationLerpSpeed * Time.fixedDeltaTime * 2f);
            float angularSpeed = (_currentSpeed / (0.38f * (float) Math.PI) * 360f) % 360f;
            foreach (var turningWheel in turningWheels)
            {
                turningWheel.localEulerAngles = Vector3.up * (_lerpedWheelDirection * KartPhysicsSettings.instance.kartWheelAngle);
            }

            /*foreach (var wheel in wheels)
            {
                wheel.localEulerAngles = Vector3.right * angularSpeed;
            }*/
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

        public float GetHorizontalAxis()
        {
            return hMove;
        }
    }
}