using System;
using System.Collections;
using System.Collections.Generic;
using Kart;
using UnityEngine;

[RequireComponent(typeof (KartBase))]
public class KartAudio : MonoBehaviour
{
    public Camera cam;
    public KartBase kart;
    public AudioClip audioClip;
    public float pitchMultiplier = 1f;
    public float maxRolloffDistance = 500f;
    public bool useDoppler = true;

    private AudioSource _HighAccel;
    private bool _StartedSound;
    private float _speed;
    
    private void StartSound()
    {
        _HighAccel = SetupEngineAudioSource(audioClip);
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
            source.Play();
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
            Debug.Log("PLAY SOUND");
            StartSound();
        }

        if (_StartedSound)
        {
            _speed = Mathf.Lerp(_speed, kart.CurrentSpeed(), AudioSettings.instance.kartSpeedLerp*Time.deltaTime);
            float pitch = Mathf.Lerp(AudioSettings.instance.kartPitchMin, AudioSettings.instance.kartPitchMax, _speed / AudioSettings.instance.kartMaxSpeed);

            pitch = Mathf.Min(AudioSettings.instance.kartPitchMax, pitch);
            _HighAccel.pitch = pitch * pitchMultiplier;
            _HighAccel.volume = AudioSettings.instance.motorVolume;
        }
        
    }
}
