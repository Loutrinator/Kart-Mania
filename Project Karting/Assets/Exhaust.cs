using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhaust : MonoBehaviour
{
    public List<Transform> exhausts;
    public AnimationCurve exhaustsPositionAC;
    public AnimationCurve exhaustsRotationAC;
    public float speed = 1f;

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
    }
}
