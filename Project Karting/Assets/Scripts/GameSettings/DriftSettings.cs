using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityExtendedEditor.ExtendedAttributes.Editor;

[CreateAssetMenu(fileName = "DriftSettings", menuName = "ScriptableObjects/DriftSettings")]
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
    public float boostFOVOffset;
    public ShakeTransformEventData boost1Shake;
    public ShakeTransformEventData boost2Shake;
    public ShakeTransformEventData boost3Shake;

    #endregion
}
