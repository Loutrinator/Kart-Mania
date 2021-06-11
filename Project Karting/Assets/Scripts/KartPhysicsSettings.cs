using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsSettings", menuName = "ScriptableObjects/PhysicsSettings")]
public class KartPhysicsSettings : ScriptableObject
{
    
    #region Singleton
    public static KartPhysicsSettings instance;
        
    private void OnEnable()
    {
        if (instance != null)
            throw new UnityException(typeof(KartPhysicsSettings) + " is already instantiated");
        instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }
    #endregion
    #region Kart Stats
    public float topSpeed = 40f;
    public float acceleration = 20f;
    public float reverseSpeed = 15;
    public float reverseAcceleration = 5f;
    public float braking = 15;
    #endregion
    #region Kart physics
    public float steeringSpeed = 40f;
    public float minDriftAngle = 0.182f;
    public float maxDriftAngle = 1.8f;
    public float kartRotationCoeff = 15f;
    public float kartRollCoeff = 4f;
    public float kartRotationLerpSpeed = 5f;
    public float kartWheelAngle = 25f;
    public float boostLength = 2f;
    public float boostStrength = 1f;
    public float engineBrakeSpeed = 10f;

    #endregion
}
