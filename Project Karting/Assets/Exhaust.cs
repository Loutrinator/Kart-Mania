using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Exhaust : MonoBehaviour
{
    public KartBase kart;
    public List<Transform> exhausts;
    public List<ParticleSystem> smokeEmitters;
    [Header("Animation")]
    public AnimationCurve exhaustsPositionAC;
    public AnimationCurve exhaustsRotationAC;
    
    public float speed = 1f;

    private bool wasEmmiting = false;

    void Update()
    {
        float time = (Time.time*speed)%1;
        float pos = exhaustsPositionAC.Evaluate(time);
        float rot = exhaustsRotationAC.Evaluate(time);
        foreach (var exhaust in exhausts)
        {
            Vector3 oldPos = exhaust.localPosition;
            exhaust.localPosition = new Vector3(oldPos.x,pos,oldPos.z);
            Vector3 oldRot = exhaust.localEulerAngles;
            exhaust.localEulerAngles = new Vector3(rot,oldRot.y,oldRot.z);
        }

        bool emit = Mathf.Abs(kart.currentSpeed()) < 0.1f;
        if (emit) {
            if (!wasEmmiting) {
                foreach (var smoke in smokeEmitters)
                {
                    smoke.Play();
                }
                wasEmmiting = true;   
            }
        }
        else {
            foreach (var smoke in smokeEmitters)
            {
                smoke.Stop();
            }
            wasEmmiting = false;
        }
    }
}
