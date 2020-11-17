using System;
using System.Collections.Generic;
using UnityEngine;

public class KartBase : MonoBehaviour {
    public Rigidbody rigidBody;
    public Vector3 roadDirection = Vector3.up;
    public Transform raycastCenter;    // Use for gravity & ground check
    public Transform kartRootModel;        // The kart's root 3D model
    public Transform kartBodyModel;        // The main kart 3D model (no wheels)
    public List<Transform> wheels;        // The wheels of the kart
    public List<Transform> turningWheels;        // The turning wheels of the kart
    public Transform rotationAxis;
    public KartEffects effects;

    public Stats vehicleStats = new Stats {
        topSpeed = 10f,    //
        acceleration = 5f,    //
        braking = 10f,
        reverseAcceleration = 5f,    //
        reverseSpeed = 5f,
        steer = 5f,
        addedGravity = 1f,
        suspension = .2f
    };
    
    public float steeringSpeed = 70f;
    
    [Header("Drift")]
    [Range(0f, 1f)] public float minDriftAngle = 0.2f;
    [Range(1f, 2f)] public float maxDriftAngle = 2f; 
    public float kartRotationCoeff = 10f; //how much the 3d model turns
    public float kartRollCoeff = 10f; //how much the 3d model rolls
    public float kartRotationLerpSpeed = 1f; //how much the 3d model turns
    public float kartWheelAngle = 15f; //how much the wheels turn

    public float boostLength = 2f;
    public float boostStrength = 1f;
    
    protected float hMove;
    protected float lerpedAngle;
    protected int forwardMove;    // -1; 0; 1
    protected bool drift;
    protected int driftDirection;
    private bool drifting;

    [HideInInspector] public PlayerRaceInfo raceInfo;
    
    private Vector3 _firstPos;
    private float _firstPosTime;
    private float _currentSpeed;
    private float _currentAngularSpeed;
    private float _lerpedWheelDirection;
    private float _lerpedKartRotation;
    private void Awake() {
        _firstPos = transform.position;
        _firstPosTime = Time.time;
        stopDrifting();
    }
    
    private void FixedUpdate()
    {
        if (!GameManager.Instance.raceHasBegan()) return;
        move(forwardMove);
        animateWheels();
        
        if (drift && !drifting && (hMove < 0 ||hMove > 0))
        {
            //Debug.Log("Start drift");
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
    }
    protected void move(float direction) {
        if (direction > 0) {
            _currentSpeed += vehicleStats.acceleration * Time.fixedDeltaTime;
            _currentSpeed = Mathf.Min(vehicleStats.topSpeed, _currentSpeed);
        }
        else if (direction < 0) {
            _currentSpeed -= vehicleStats.reverseAcceleration * Time.fixedDeltaTime;
            _currentSpeed = Mathf.Max(-vehicleStats.reverseSpeed, _currentSpeed);
        }
        else _currentSpeed = 0;

        var t = transform;
        t.position += t.forward * (_currentSpeed * Time.fixedDeltaTime);
    }

    protected void rotate(float angle)
    {
        lerpedAngle = Mathf.Lerp(lerpedAngle, angle, kartRotationLerpSpeed * Time.fixedDeltaTime);
        float steerAngle = lerpedAngle * (vehicleStats.steer*2 + steeringSpeed) * Time.fixedDeltaTime;
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
    
    public float currentSpeed() {
        return _currentSpeed;
    }
    
    public Vector3 getRoadDirection()
    {
        return roadDirection;
    }
    public float getHorizontalAxis()
    {
        return hMove;
    }
}