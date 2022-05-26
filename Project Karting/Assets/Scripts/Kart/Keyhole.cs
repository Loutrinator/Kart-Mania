using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using UnityEngine;

public class Keyhole : MonoBehaviour
{
    public bool ANIMATION;
    public float baseSpeed = 300f;
    public float lerpSpeed = 2f;
    public Transform keyhole;
    public GameObject key;
    public MeshRenderer keyMeshRenderer;
    public float keyPositionOffset = -0.2f;

    public enum RewindMode
    {
        auto, startRewindMode, manual
    }
    [SerializeField]
    public RewindMode rewindMode;

    public float rewindDuration = 1f;
    public float idleDuration = 3f;
    
    [Header("Key insertion animation")]
    public float insertionDuration = 1f;
    public AnimationCurve insertionPositionAC;
    public AnimationCurve insertionRotationAC;
    public AnimationCurve insertionScaleAC;

    [Header("Key extraction animation")]
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
    
    [Header("Sound")]
    
    [SerializeField] private AudioSource keyPopSound;
    [SerializeField] private AudioSource ratchetSound;
    [SerializeField] private AudioSource keyInsertion;
    
    
    private float currentSpeed;
    private Action powerupCallback;
    private enum KeyHoleState
    {
        empty,insertion,rewind,inserted,extraction
    }
    private KeyHoleState keyHoleState;
    private float currentStateStartTime;
    private float _idleDuration;
    private bool _insertionSoundPlayed;
    
    private void Awake()
    {
        _idleDuration = idleDuration;
    }

    private void Start()
    {
        currentSpeed = 0f;
        keyHoleState = KeyHoleState.empty;
    }

    void Update()
    {
        if (ANIMATION)
        {
            ANIMATION = false;
            InsertKey(rewindMode, null);
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
                elapsed = Time.time - currentStateStartTime;
                Vector3 oldRotation = keyhole.localEulerAngles;
                float newZRotation = oldRotation.z + currentSpeed * Time.deltaTime;
                newZRotation = newZRotation % 360f;
                keyhole.localEulerAngles = new Vector3(oldRotation.x, oldRotation.y, newZRotation);
                if (elapsed >= _idleDuration)
                {
                    currentStateStartTime = Time.time;
                    keyHoleState = KeyHoleState.extraction;
                }
                break;
            
            // ------ REWIND STATE ------
            case KeyHoleState.rewind:
                elapsed = Time.time - currentStateStartTime;
                float newZR;
                switch (rewindMode)
                {
                    // ------ AUTO REWIND ------
                    case RewindMode.auto:
                        float autoSpeed = autoRewindSpeed * autoRewindSpeedAC.Evaluate(elapsed/autoRewindTimeToMaxSpeed);
                        newZR = keyhole.localEulerAngles.z - autoSpeed * Time.deltaTime;
                        keyhole.localEulerAngles = new Vector3(0, 0, +newZR);
                        break;
                    
                    // ------ HAND REWIND ------
                    case RewindMode.startRewindMode:
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
                    case RewindMode.manual:
                        break;
                }

                if (elapsed >= rewindDuration)
                {
                    if (rewindMode != RewindMode.startRewindMode)
                    {
                        currentStateStartTime = Time.time;
                        keyHoleState = KeyHoleState.inserted;
                        
                        if (powerupCallback != null)
                        {
                            powerupCallback();
                        } 
                    }
                }
                break;
            
            // ------ INSERTION STATE ------
            case KeyHoleState.insertion:
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
                    if (!_insertionSoundPlayed)
                    {
                        keyInsertion.Play();
                        _insertionSoundPlayed = true;
                    }
                    if (rewindMode != RewindMode.startRewindMode)
                    {
                        currentStateStartTime = Time.time;
                        keyHoleState = KeyHoleState.rewind;
                        _insertionSoundPlayed = false;
                    }
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
                    currentStateStartTime = Time.time;
                    keyHoleState = KeyHoleState.empty;
                    keyMeshRenderer.enabled = false;
                    _insertionSoundPlayed = false;
                }
                break;
        }
    }

    private IEnumerator HoldKeyPosition()
    {
        yield return new WaitForSeconds(handRewindWait);
        HandRewindHoldPosition = false;
        ratchetSound.Play();
    }

    public bool InsertKey(RewindMode mode, [CanBeNull] Action callback)
    {
        
        _insertionSoundPlayed = false;
        
        rewindMode = mode;
        if (keyHoleState == KeyHoleState.empty)
        {
            currentStateStartTime = Time.time;
            keyHoleState = KeyHoleState.insertion;
            keyPopSound.Play();
            ResetKeyhole();
            keyMeshRenderer.enabled = true;
            if (callback != null)
            {
                powerupCallback = callback;
            }
            return true;
        }
        return false;
    }
    
    public void RemoveKey()
    {
        currentStateStartTime = Time.time;
        keyHoleState = KeyHoleState.extraction;
    }
    void ResetKeyhole()
    {
        Vector3 oldRotation = keyhole.eulerAngles;
        keyhole.eulerAngles = new Vector3(oldRotation.x, oldRotation.y, 0);
    }

    public void Rewind()
    {
        if (rewindMode == RewindMode.startRewindMode && keyHoleState == KeyHoleState.insertion)
        {
            currentStateStartTime = Time.time;
            keyHoleState = KeyHoleState.rewind;
            ratchetSound.Play();
        }
    }
    public void StopRewind(float duration)
    {
        if (rewindMode == RewindMode.startRewindMode && keyHoleState == KeyHoleState.rewind)
        {
            _idleDuration = duration;
            currentStateStartTime = Time.time;
            keyHoleState = KeyHoleState.inserted;
                        
            if (powerupCallback != null)
            {
                powerupCallback();
            }
        }
    }
}
