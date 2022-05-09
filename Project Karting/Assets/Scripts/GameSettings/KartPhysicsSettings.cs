using UnityEngine;
using UnityExtendedEditor.Editor;

[CreateAssetMenu(fileName = "KartPhysicsSettings", menuName = "ScriptableObjects/Settings/KartPhysicsSettings")]
public class KartPhysicsSettings : ScriptableObject
{
    
    #region Singleton
    public static KartPhysicsSettings instance;
        
    private void OnEnable()
    {
        if (instance == null) instance = this;
    }

    private void OnDisable()
    {
        instance = null;
    }
    #endregion
    #region Kart Stats
    [Header("Kart's stats")]
    [SerializeField][MinMaxSlider(0f,300f)]
    private Vector2 topSpeed = new Vector2(150,200f);
    [SerializeField][MinMaxSlider(0f,150f)]
    private Vector2 acceleration = new Vector2(50,125f);
    [SerializeField][MinMaxSlider(0f,65f)]
    private Vector2 steeringSpeed = new Vector2(20,45f);
    [SerializeField][MinMaxSlider(0f,25f)]
    private Vector2 weight = new Vector2(6,18f);
    [SerializeField][MinMaxSlider(0f,1f)]
    private Vector2 minDriftAngle = new Vector2(0.1f,0.25f);
    [SerializeField][MinMaxSlider(0f,2f)]
    private Vector2 maxDriftAngle =new Vector2(1.25f,1.5f);

    public float getTopSpeed(float stat)
    {
        return getStat(stat, topSpeed);
    }
    public float getAcceleration(float stat) { return getStat(stat, acceleration); }
    public float getSteeringSpeed(float stat) { return getStat(stat, steeringSpeed); }
    public float getGravity(float stat) { return getStat(stat, weight); }
    public float getMinDrift(float stat) { return getStat(1-stat, minDriftAngle); }
    public float getMaxDrift(float stat) { return getStat(stat, maxDriftAngle); }
    private float getStat(float value, Vector2 range)
    {
        return range.x + (range.y - range.x) * value;
    }
    
    #endregion
    #region Kart physics
    [Header("Kart's physics")]
    public float reverseSpeedCoeff = 0.5f;
    public float reverseAccelerationCoeff = 3f;
    public float braking = 15;
    public float kartRotationCoeff = 15f;
    public float kartModelRotationCoeff = 15f;
    public float kartRollCoeff = 4f;
    public float kartRotationLerpSpeed = 5f;
    public float kartWheelAngle = 25f;
    public float boostStrength = 1f;
    public float engineBrakeSpeed = 10f;
    public float respawnHeight = 2f;
    public float respawnMinDistance = 35f;
    public float borderVelocityLossPercent = 0.2f;
    public float bumpForce = 3000f;
    public float minVelocityToTurn = 0.2f;
    public float minVelocityToDrift = 10f;

    #endregion
    #region Camera behaviour
    [Header("Camera behaviour")]
    public float cameraPositionLerp = 10f;
    public float cameraRotationLerp = 10f;
    public float cameraSideAmplitude = 0.5f;
    public float cameraSideAngleAmplitude = 0.5f;
    public float cameraKartDirectionLerp = 0.5f;
    #endregion
}
