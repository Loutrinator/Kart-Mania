using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Keyhole : MonoBehaviour
{

    public float baseSpeed = 300f;
    public float lerpSpeed = 2f;
    public Transform keyhole;
    public GameObject key;
    public MeshRenderer keyMeshRenderer;
    public float keyPositionOffset = -0.2f;

    public enum RewindSTATE
    {
        auto, hand, manual
    }
    [SerializeField]
    public RewindSTATE rewindSTATE;
    
    [Header("Key insertion animation")]
    public bool insertKey = false;
    public float insertionDuration = 1f;
    public AnimationCurve insertionPositionAC;
    public AnimationCurve insertionRotationAC;
    public AnimationCurve insertionScaleAC;

    [Header("Key extraction animation")]
    public bool extractKey = false;
    public float extractionDuration = 1f;
    public AnimationCurve extractionPositionAC;
    public AnimationCurve extractionRotationAC;

    [Header("Key Hand rewind animation")]
    public float handRewindDuration = 1f;
    public float handRewindWait = 1f;
    public float handRewindStopAngle = 180f;
    private bool HandRewindHoldPosition;
    
    [Header("Key auto rewind animation")]
    public float autoRewindSpeed = 500f;
    public float autoRewindTimeToMaxSpeed = 0.3f;
    public AnimationCurve autoRewindSpeedAC;

    private float currentSpeed;
    private enum KeyHoleState
    {
        empty,insertion,rewind,inserted,extraction
    }
    private KeyHoleState keyHoleState;
    private float currentStateStartTime;
    private void Start()
    {
        currentSpeed = 0f;
        keyHoleState = KeyHoleState.empty;
    }

    void Update()
    {
        if (insertKey)
        {
            insertKey = false;
            InsertKey();
        }
        if (extractKey)
        {
            extractKey = false;
            RemoveKey();
        }

        currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, lerpSpeed * Time.deltaTime);
        float animationPercent;
        float elapsed;
        float position;
        Vector3 newPos;
        float rotation;
        Vector3 newRot;
        switch (keyHoleState)
        {
            // ------ EMPTY STATE ------
            case KeyHoleState.empty:
                break;
            
            // ------ INSERTED STATE ------
            case KeyHoleState.inserted:
                Vector3 oldRotation = keyhole.localEulerAngles;
                float newZRotation = oldRotation.z + currentSpeed * Time.deltaTime;
                newZRotation = newZRotation % 360f;
                keyhole.localEulerAngles = new Vector3(oldRotation.x, oldRotation.y, newZRotation);
                break;
            
            // ------ REWIND STATE ------
            case KeyHoleState.rewind:
                elapsed = Time.time - currentStateStartTime;
                float newZR;
                switch (rewindSTATE)
                {
                    // ------ AUTO REWIND ------
                    case RewindSTATE.auto:
                        float autoSpeed = autoRewindSpeed * autoRewindSpeedAC.Evaluate(elapsed/autoRewindTimeToMaxSpeed);
                        newZR = keyhole.localEulerAngles.z - autoSpeed * Time.deltaTime;
                        keyhole.localEulerAngles = new Vector3(0, 0, +newZR);
                        break;
                    
                    // ------ HAND REWIND ------
                    case RewindSTATE.hand:
                        if (!HandRewindHoldPosition)
                        {
                            Vector3 oldR = keyhole.localEulerAngles;
                            float speed = handRewindStopAngle / handRewindDuration;
                            newZR = oldR.z - speed * Time.deltaTime;
                            if (newZR <= 0)
                            {
                                HandRewindHoldPosition = true;
                                StartCoroutine(HoldKeyPosition());
                                newZR = 180f;
                            }
                            keyhole.localEulerAngles = new Vector3(oldR.x, oldR.y, +newZR);
                        }
                        break;
                    
                    // ------ MANUAL REWIND ------
                    case RewindSTATE.manual:
                        break;
                }
                
                break;
            
            // ------ INSERTION STATE ------
            case KeyHoleState.insertion:
                Debug.Log("INSERTION");
                elapsed = Time.time - currentStateStartTime;
                animationPercent = elapsed / insertionDuration;
                
                position = insertionPositionAC.Evaluate(animationPercent);
                newPos = new Vector3(0,0,position+keyPositionOffset);
                // Convertit la position en référentiel world space
                newPos = keyhole.transform.TransformPoint(newPos);
                key.transform.position =newPos;

                rotation = insertionRotationAC.Evaluate(animationPercent);
                newRot = new Vector3(0,0,rotation);
                // Convertit la position en référentiel world space
                //newRot = keyhole.transform.Transfor (newPos);
                key.transform.localEulerAngles =newRot;
                if (elapsed >= insertionDuration)
                { 
                    keyHoleState = KeyHoleState.rewind;
                }
                break;
            
            // ------ EXTRACTION STATE ------
            case KeyHoleState.extraction:
                elapsed = Time.time - currentStateStartTime;
                animationPercent = elapsed / extractionDuration;
                position = extractionPositionAC.Evaluate(animationPercent);
                newPos = new Vector3(0,0,position+keyPositionOffset);
                // Convertit la position en référentiel world space
                newPos = keyhole.transform.TransformPoint(newPos);
                key.transform.position =newPos;

                rotation = extractionRotationAC.Evaluate(animationPercent);
                newRot = new Vector3(0,0,rotation);
                // Convertit la position en référentiel world space
                //newRot = keyhole.transform.Transfor (newPos);
                key.transform.localEulerAngles =newRot;
                if (elapsed >= extractionDuration)
                {
                    keyHoleState = KeyHoleState.empty;
                    keyMeshRenderer.enabled = false;
                }
                break;
        }
    }

    private IEnumerator HoldKeyPosition()
    {
        yield return new WaitForSeconds(handRewindWait);
        HandRewindHoldPosition = false;
    }

    void InsertKey()
    {
        if (keyHoleState == KeyHoleState.empty)
        {
            currentStateStartTime = Time.time;
            keyHoleState = KeyHoleState.insertion;
            ResetKeyhole();
            keyMeshRenderer.enabled = true;
        }
    }
    
    void RemoveKey()
    {
        currentStateStartTime = Time.time;
        keyHoleState = KeyHoleState.extraction;
    }
    void ResetKeyhole()
    {
        Vector3 oldRotation = keyhole.eulerAngles;
        keyhole.eulerAngles = new Vector3(oldRotation.x, oldRotation.y, 0);
    }
}
