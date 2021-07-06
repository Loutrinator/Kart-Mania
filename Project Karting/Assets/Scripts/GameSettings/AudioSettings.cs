using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "ScriptableObjects/KartAudioSettings")]
public class AudioSettings : ScriptableObject
{
    
    #region Singleton
    public static AudioSettings instance;
        
    private void OnEnable()
    {
        if (instance == null)
            instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }
    #endregion

    [Header("Volume mixer")] 
    [Range(0f, 1f)]public float musicVolume = 0.5f;
    [Range(0f, 1f)]public float UIVolume = 0.5f;
    [Range(0f, 1f)]public float motorVolume = 0.5f;
    [Range(0f, 1f)]public float driftVolume = 0.5f;
    #region Kart
    public float kartSpeedLerp = 1.5f;
    public float kartPitchMin = 0.8f;
    public float kartPitchMax = 1.6f;
    public float kartMaxSpeed = 100;
    #endregion
}
