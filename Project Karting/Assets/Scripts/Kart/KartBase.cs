using System;
using System.Collections.Generic;
using System.Data;
using Items;
using UnityEngine;

namespace Kart
{
    public class KartBase : MonoBehaviour {
        public Transform kartRootModel;        // The kart's root 3D model
        public Transform kartBodyModel;        // The main kart 3D model (no wheels)
        public List<Transform> wheels;        // The wheels of the kart
        public List<Transform> turningWheels;        // The turning wheels of the kart

        public Transform rotationAxis;
        public KartEffects effects;
        public Keyhole keyhole;
        public Stats vehicleStats = new Stats {
            topSpeed = 10f,   
            acceleration = 5f,  
            braking = 10f,
            reverseAcceleration = 5f, 
            reverseSpeed = 5f,
            steer = 5f,
            addedGravity = 1f,
            suspension = .2f
        };
        
        List<StatPowerup> activePowerupList = new List<StatPowerup>();
        private Stats finalStats;

        public float steeringSpeed = 70f;
        public float yAxisOffset = 1f;
        [Header("Drift")]
        [Range(0f, 1f)] public float minDriftAngle = 0.2f;
        [Range(1f, 2f)] public float maxDriftAngle = 2f; 
        public float kartRotationCoeff = 10f; //how much the 3d model turns
        public float kartRollCoeff = 10f; //how much the 3d model rolls
        public float kartRotationLerpSpeed = 1f; //how much the 3d model turns
        public float kartWheelAngle = 15f; //how much the wheels turn

        public float boostLength = 2f;
        public float boostStrength = 1f;

        public float hMove;
        protected float lerpedAngle;
        public int forwardMove;    // -1; 0; 1
        public bool drift;
        protected int driftDirection;
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

        private void Awake() {
            _firstPos = transform.position;
            _firstPosTime = Time.time;
            stopDrifting();
        }
	    
        private void FixedUpdate() {
            if (!GameManager.Instance.raceHasBegan()) return;
            applyPowerups();
	        move(forwardMove);
            animateWheels();

            if (drift && !drifting && (hMove < 0 ||hMove > 0))
            {
                #if UNITY_EDITOR
                Debug.Log("Start drift");
                #endif
                startDrift(hMove);
            }

            if (forwardMove != 0) //on tourne pas à l'arret
            {
                if (drifting){
                    if (!drift)
                    {
                        stopDrifting();
                    }else{   
                        float driftAngle = (1 + hMove*driftDirection)  / 2 * (maxDriftAngle - minDriftAngle) + minDriftAngle;
                        driftAngle *= driftDirection;
                        rotate(driftAngle);
                    } 
                }else{
                    rotate(hMove);
                }
            }
            applyGravity();
            //var t = transform;
            //t.position += t.forward * (_currentSpeed * Time.fixedDeltaTime);
        }


        protected void move(float direction) {
            if (direction > 0) {
                _currentSpeed += finalStats.acceleration * Time.fixedDeltaTime;
                _currentSpeed = Mathf.Min(finalStats.topSpeed, _currentSpeed);
            }
            else if (direction < 0) {
                _currentSpeed -= finalStats.reverseAcceleration * Time.fixedDeltaTime;
                _currentSpeed = Mathf.Max(-finalStats.reverseSpeed, _currentSpeed);
            }
            else _currentSpeed = 0;

            var t = transform;
            t.position += t.forward * (_currentSpeed * Time.fixedDeltaTime);
        }

        protected void rotate(float angle)
        {
            lerpedAngle = Mathf.Lerp(lerpedAngle, angle, kartRotationLerpSpeed * Time.fixedDeltaTime);
            float steerAngle = lerpedAngle * (finalStats.steer*2 + steeringSpeed) * Time.fixedDeltaTime;
            transform.RotateAround(rotationAxis.position, rotationAxis.up, steerAngle);
            Vector3 currentRotation = kartRootModel.rotation.eulerAngles;
            
            
            /*
            _lerpedKartRotation = Mathf.Lerp(lerpedAngle, driftDirection, kartRotationLerpSpeed * Time.fixedDeltaTime);

            if (drifting)
            {   
                kartRootModel.localEulerAngles = Vector3.up * (_lerpedKartRotation * 70 * kartRotationCoeff);
            }
            else
            {
                kartRootModel.localEulerAngles = Vector3.up * (steerAngle * kartRotationCoeff);
            }*/
            kartRootModel.localEulerAngles = Vector3.up * (steerAngle * kartRotationCoeff);

            kartBodyModel.localEulerAngles = Vector3.forward * (steerAngle * kartRollCoeff);
        }

        private void applyGravity() {
            Transform t = transform;
            if (Physics.Raycast(t.position + t.up * 1f, -t.up, out var hit, 1.1f+yAxisOffset,
                1 << LayerMask.NameToLayer("Ground"))) {
                t.position += (hit.point.y - t.position.y+yAxisOffset) * t.up;
                _yVelocity = 0;
            }
            else {
                _yVelocity += Physics.gravity.y * Time.fixedDeltaTime;
            }
            transform.position += transform.up * (_yVelocity * Time.fixedDeltaTime);
        }
        
        private void animateWheels()
        {
            _lerpedWheelDirection = Mathf.Lerp(_lerpedWheelDirection, hMove, kartRotationLerpSpeed * Time.fixedDeltaTime * 2f);
            float angularSpeed = (_currentSpeed /(0.38f * (float)Math.PI)*360f)%360f;
            foreach (var turningWheel in turningWheels)
            {
                turningWheel.localEulerAngles = Vector3.up * (_lerpedWheelDirection * kartWheelAngle);
            }
            foreach (var wheel in wheels)
            {
                //wheel.localEulerAngles = Vector3.right * angularSpeed;
            }
        }

        #region Drift
        private void startDrift(float direction)
        {
            driftDirection = direction > 0 ? 1 : direction < 0 ? -1 : 0;
            drifting = true;
            effects.startDrift();
        }

        private void stopDrifting()
        {
            driftDirection = 0;
            _lerpedKartRotation = 0f;
            drifting = false;
            effects.stopDrift();
            effects.startBoost(boostLength,boostStrength);
        }
        #endregion

        #region PowerUps
        void applyPowerups()
        {
            // on supprime tout powerup qui a dépassé son temps d'activation
            activePowerupList.RemoveAll((p) =>
            {
                if (p.elapsedTime > p.maxTime)
                {
                    p.powerupUsed();
                    return true;   
                }
                return false;
            });
            var powerups = new Stats();// on initialise des stats vierges pour nos powerups
            // on ajoute à 'powerups' les modifiers de chaque powerup
            for (int i = 0; i < activePowerupList.Count; i++)
            {
                var p = activePowerupList[i];
                // on met a jour le compteur de temps écoulé depuis l'obtention du powerup
                p.elapsedTime += Time.deltaTime;
                // on additionne les modifications des stats de notre powerup à 'powerups'
                powerups += p.modifiers;
            }

            // on ajoute tous nos powerups cumulés à nos stats de base du véhicule
            finalStats = vehicleStats + powerups;

            // on clamp toutes les valeurs des stats qui nécessitent de pas dépasser [0,1]
            finalStats.suspension = Mathf.Clamp(finalStats.suspension, 0, 1);
        }

        public void addPowerup(StatPowerup powerup)
        {
            activePowerupList.Add(powerup);
        }
        #endregion
        
        public float currentSpeed() {
            return _currentSpeed;
        }

        public float getHorizontalAxis()
        {
            return hMove;
        }
    }
}