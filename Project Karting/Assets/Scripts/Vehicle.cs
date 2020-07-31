using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Vehicle : MonoBehaviour
{
    public Motor motor;
    public Transform centerOfMass;

    //wheels 
    
    public WheelCollider[] wheelColliders;
    public Transform[] wheelTransforms;
    public bool[] wheelIsMotrice;
    public bool[] wheelIsSteering;
    public bool[] wheelIsReverseSteering;

    //[HideInInspector]

    public float motorTorque = 100f;
    public float brakeTorque = 100f;
    public float maxSteer = 20f;
    private Rigidbody _rigidbody;
    private Vector3 previousPosition;
    private float wheelsRPM;
    private bool reverse;
    public bool isBracking; //If the brakes are activated
    public float Speed
    {
        get { return _rigidbody.velocity.magnitude * 3.6f; }
    }
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.localPosition;
        previousPosition = transform.position;
    }
    
    void Update()
    {
        steerWheels();
        checkBraking();
        applyWheelsPhysics();
        
       
        wheelColliders[2].motorTorque = Input.GetAxis("Vertical") * motorTorque;
        wheelColliders[3].motorTorque = Input.GetAxis("Vertical") * motorTorque;
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            //wheelColliders[i].brakeTorque =  Input.GetAxis("Brake") * brakeTorque;
            
            wheelColliders[i].GetWorldPose(out pos,out rot);
            wheelTransforms[i].position = pos;
            wheelTransforms[i].rotation = rot * Quaternion.Euler(0,90,0);
            if (i % 2 == 0) wheelTransforms[i].rotation *= Quaternion.Euler(0, 180, 0);
        }
        
        float dist = (previousPosition - transform.position).magnitude;
        previousPosition = transform.position;
    }

    private void steerWheels()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (wheelIsSteering[i])
            {
                var steerAngle = Input.GetAxis("Horizontal") * maxSteer;
                if (wheelIsReverseSteering[i]) steerAngle *= -1;
                wheelColliders[i].steerAngle = steerAngle;
            }
        }
    }
    private void checkBraking()
    {
        if (Input.GetKey("space"))
        {
            Debug.Log("BRAKE");
            isBracking = true;
        }
        else isBracking = false; 
    }
    private void applyWheelsPhysics()
    {
        wheelColliders[2].motorTorque = Input.GetAxis("Vertical") * motorTorque;
        wheelColliders[3].motorTorque = Input.GetAxis("Vertical") * motorTorque;
        var pos = Vector3.zero;
        var rot = Quaternion.identity;
        
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            
            if(wheelIsMotrice[i]) wheelColliders[i].motorTorque = motor.Torque;
            wheelColliders[i].GetWorldPose(out pos,out rot);
            wheelTransforms[i].position = pos;
            wheelTransforms[i].rotation = rot * Quaternion.Euler(0,90,0);
            if (i % 2 == 0) wheelTransforms[i].rotation *= Quaternion.Euler(0, 180, 0);
        }
    }
    
    
    public float wheelRPM(){
        float sum = 0;
        int R = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheelColliders[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;
 
        if(wheelsRPM < 0 && !reverse ){
            reverse = true;
        }
        else if(wheelsRPM > 0 && reverse){
            reverse = false;
        }

        return wheelsRPM;
    }
}
