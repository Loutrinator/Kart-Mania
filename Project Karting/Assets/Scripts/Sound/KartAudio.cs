using System;
using System.Collections;
using System.Collections.Generic;
using Kart;
using UnityEngine;

[RequireComponent(typeof (KartBase))]
public class KartAudio : MonoBehaviour
{
    [HideInInspector] public Camera cam;
    [HideInInspector] public KartBase kart;
    public AudioClip lowAccelerationClip;
    public AudioClip lowDecelerationClip;
    public AudioClip highAccelerationClip;
    public AudioClip highDecelerationClip;
    public float pitchMultiplier = 1f;
    public float lowPitchMin = 1f;
    public float lowPitchMax = 6f;
    public float highPitchMultiplier = 0.25f;
    public float maxRolloffDistance = 500f;
    public float dopplerLevel = 1f;
    public bool useDoppler = true;

    private AudioSource _LowAccel;
    private AudioSource _LowDecel;
    private AudioSource _HighAccel;
    private AudioSource _HighDecel;
    private bool _StartedSound;
    
    private void StartSound()
    {
        //_LowAccel = SetupEngineAudioSource(lowAccelerationClip);
        //_LowDecel = SetupEngineAudioSource(lowDecelerationClip);
        //_HighDecel = SetupEngineAudioSource(highDecelerationClip);
        _HighAccel = SetupEngineAudioSource(highAccelerationClip);
        _StartedSound = true;
    }

    private void StopSound()
    {
        foreach (var source in GetComponents<AudioSource>())
        {
            Destroy(source);
        }
        _StartedSound = false;
    }
    private AudioSource SetupEngineAudioSource(AudioClip audioClip)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        if (audioClip == null)
        {
            Debug.Log("NO AUDIOCLIP");
        }
        else{
            source.clip = audioClip;
            source.loop = true;
            source.priority = Int32.MaxValue;
            source.playOnAwake = true;
        }
        return source;
    }

    private void Update()
    {
        float camDist = (cam.transform.position - transform.position).sqrMagnitude;

        if (_StartedSound && camDist > maxRolloffDistance * maxRolloffDistance)
        {
            StopSound();
        }else if (!_StartedSound && camDist < maxRolloffDistance * maxRolloffDistance)
        {
            StartSound();
        }

        if (_StartedSound)
        {
            float pitch = Mathf.Lerp(lowPitchMin, lowPitchMax, kart.CurrentSpeed() / 60);

            pitch = Mathf.Min(lowPitchMax, pitch);
            _HighAccel.pitch = pitch * pitchMultiplier * highPitchMultiplier;
            _HighAccel.dopplerLevel = useDoppler ? dopplerLevel : 0;
            _HighAccel.volume = 1;
        }
        
    }
}
