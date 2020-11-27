using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class Motor : MonoBehaviour
{   
    
    public int numberOfGears;
    public float minRpm;
    public float maxRpm;
    public float[] gearMaxRpm;
    public float[] gearMaxSpeed;
    public AnimationCurve motorTorqueCurve;

    public float Torque => motorTorqueCurve.Evaluate(Rpm);
    public float Rpm => vehicle.wheelRPM(); //TEMPORAIRE

    private AudioSource engineSound;
    public float engineSoundSpeed = 200f;
    private Vehicle vehicle;
    public float minPitch;
    public float maxPitch;

    private float throttle;
    public float Throttle
    {
        get { return throttle; }
        set
        {
            if (value >= 1) throttle = 1;
            else if (value <= 0) throttle = 0;
            else throttle = value;
        }
    }
    
    private int currentGear = 1;
    public int CurrentGear
    {
        get { return currentGear; }
        
    }

    private void Start()
    {
        vehicle = gameObject.GetComponent<Vehicle>();
        engineSound = gameObject.GetComponent<AudioSource>();
        
    }

    public void refreshGear(float speed)
    {
        //if(gearMaxSpeed.Length > 0 && gearMaxSpeed[0] > speed)
        for (int i = 0; i < gearMaxSpeed.Length; i++)
        {   
            if(i == gearMaxSpeed.Length-1){
                currentGear = i + 1;
                return;
            }

            if (!(gearMaxSpeed[i] > speed)) continue;
            currentGear = i + 1;
            return;
        }
        currentGear = 0;
    }
    public float getRPMForSpeed(float speed)
    {
        if (currentGear <= 0)
        {
            return minRpm;
        }

        var i = currentGear - 1;
        return speed * gearMaxRpm[i] / gearMaxSpeed[i];
    }

    public void pitchEngineSound()
    {
        float diffRPM = maxRpm - minRpm;
        float diffPitch = maxPitch - minPitch;
        float currentRPM = getRPMForSpeed(vehicle.Speed);
        float newPitch = currentRPM * diffPitch / diffRPM + minPitch;


        engineSound.pitch = Mathf.Lerp(engineSound.pitch, newPitch, 0.25f);

    }
    private void Update()
    {
        refreshGear(vehicle.Speed);
        pitchEngineSound();
    }
}
