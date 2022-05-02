using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DriftSettings", menuName = "ScriptableObjects/Settings/DriftSettings")]
public class DriftSettings : ScriptableObject
{
    
    #region Singleton
    public static DriftSettings instance;
        
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
    #region Drift
    [ColorUsage(false, false)]
    public Color[] driftEffectColors;
    public float[] driftEffectIntensity;
    public float boostSwitchSpeed;
    public float boostTimeToSwitch;
    #endregion
    
    #region Boost
    public List<ShakeTransformEventData> boostShake;
    public List<float> boostDuration;
    public List<float> boostStrength;
    #endregion
    #region Camera FOV effect
    public float boostFOVOffset;
    public float transitionSpeed;
    public AnimationCurve boostCameraIn;
    public AnimationCurve boostCameraOut;

    #endregion
    #region Sound
    
    public AudioClip driftAudioClip;
    public float driftVolume;
    public float driftSoundAnimationSpeed;
    public AnimationCurve driftVolumeEaseIn;
    public AnimationCurve driftVolumeEaseOut;
    public float driftPitchMin;
    public float driftPitchMax;
    public AnimationCurve driftPitchEaseIn;
    public AnimationCurve driftPitchEaseOut;
    
    #endregion
}
