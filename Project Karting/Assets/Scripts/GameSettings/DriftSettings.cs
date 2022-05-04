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
    [Header("Boost")]
    public List<ShakeTransformEventData> boostShake;
    public List<float> boostDuration;
    public float boostBaseStrength;
    public List<float> driftLevelCoeff;//the coeff applied to all the effects of a drift's boost
    #endregion
    #region Camera FOV effect
    [Header("Camera boost effect")]
    public float boostFOVOffset;
    public float boostZOffset;
    public float transitionSpeed;
    public AnimationCurve boostCameraIn;
    public AnimationCurve boostCameraOut;

    #endregion
    
    #region Sound
    [Header("Sound effects")]
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
    
    #region Vibrations
    [Header("Vibrations")]
    public float driftLowVibration;
    public float driftHighVibration;
    public float boostLowVibration;
    public float boostHighVibration;
    
    #endregion
}
