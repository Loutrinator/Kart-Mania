using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyhole : MonoBehaviour
{

    public float baseSpeed = 300f;
    public float lerpSpeed = 2f;
    public Transform keyhole;
    public GameObject key;
    
    [Header("Key Insertion animation")]
    public bool insertKey = false;
    public float keyInsertionOffset = -0.2f;
    public float keyInsertionDuration = 1f;
    public AnimationCurve keyInsertionPositionAC;
    public AnimationCurve keyInsertionRotationAC;
    public AnimationCurve keyInsertionScaleAC;

    [Header("Key Extraction animation")]
    public bool extractKey = false;
    public float keyExtractionDuration = 1f;
    public AnimationCurve keyExtractionPositionAC;
    public AnimationCurve keyExtractionRotationAC;

    private float currentSpeed;
    private enum KeyHoleState
    {
        empty,insertion,inserted,extraction
    }
    private KeyHoleState keyHoleState;
    private float currentStateStartTime;
    private void Start()
    {
        currentSpeed = 0f;
    }

    void Update()
    {
        if (insertKey && keyHoleState != KeyHoleState.insertion)
        {
            currentStateStartTime = Time.time;
            keyHoleState = KeyHoleState.insertion;
            insertKey = false;
            key.gameObject.SetActive(true);
        }

        if (insertKey && keyHoleState != KeyHoleState.insertion)
        {
            currentStateStartTime = Time.time;
            keyHoleState = KeyHoleState.extraction;
            extractKey = false;
            key.gameObject.SetActive(true);
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
            case KeyHoleState.empty:
                break;
            case KeyHoleState.inserted:
                Vector3 oldRotation = keyhole.eulerAngles;
                float newZRotation = oldRotation.z + currentSpeed * Time.deltaTime;
                newZRotation = newZRotation % 360f;
                keyhole.eulerAngles = new Vector3(oldRotation.x, oldRotation.y, newZRotation);
                break;
            case KeyHoleState.insertion:
                Debug.Log("Insertion");
                elapsed = Time.time - currentStateStartTime;
                Debug.Log("elapsed : " + elapsed);
                animationPercent = elapsed / keyInsertionDuration;
                
                position = keyInsertionPositionAC.Evaluate(animationPercent);
                newPos = new Vector3(0,0,position);
                // Convertit la position en référentiel world space
                newPos = keyhole.transform.TransformPoint(newPos);
                key.transform.position =newPos;

                rotation = keyInsertionRotationAC.Evaluate(animationPercent);
                newRot = new Vector3(0,0,rotation);
                // Convertit la position en référentiel world space
                //newRot = keyhole.transform.Transfor (newPos);
                //key.transform.localEulerAngles =newRot;
                if (elapsed >= keyInsertionDuration)
                {
                    keyHoleState = KeyHoleState.inserted;
                }
                break;
            case KeyHoleState.extraction:
                Debug.Log("Insertion");
                elapsed = Time.time - currentStateStartTime;
                Debug.Log("elapsed : " + elapsed);
                animationPercent = elapsed / keyInsertionDuration;
                
                position = keyInsertionPositionAC.Evaluate(animationPercent);
                newPos = new Vector3(0,0,position);
                // Convertit la position en référentiel world space
                newPos = keyhole.transform.TransformPoint(newPos);
                key.transform.position =newPos;

                rotation = keyInsertionRotationAC.Evaluate(animationPercent);
                newRot = new Vector3(0,0,rotation);
                // Convertit la position en référentiel world space
                //newRot = keyhole.transform.Transfor (newPos);
                //key.transform.localEulerAngles =newRot;
                if (elapsed >= keyInsertionDuration)
                {
                    keyHoleState = KeyHoleState.empty;
                }
                break;
        }
    }

    void InsertKey()
    {
        if (keyHoleState == KeyHoleState.empty)
        {
            currentStateStartTime = Time.time;
            keyHoleState = KeyHoleState.insertion;
            key.gameObject.SetActive(true);
        }
    }
    
    void RemoveKey()
    {
        keyHoleState = KeyHoleState.insertion;
    }
    void ResetKeyhole()
    {
        Vector3 oldRotation = keyhole.eulerAngles;
        keyhole.eulerAngles = new Vector3(oldRotation.x, oldRotation.y, 0);
    }
}
