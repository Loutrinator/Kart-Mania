using UnityEngine;

/// <summary>
/// Contains a series tunable parameters to tweak various karts for unique driving mechanics.
/// </summary>
[System.Serializable]
public struct Stats
{
    [Header("Movement Settings")]
    [Tooltip("The maximum speed forwards")]
    [Range(0f,1f)]
    public float topSpeed;

    [Tooltip("How quickly the Kart reaches top speed.")]
    [Range(0f,1f)]
    public float acceleration;

    [Tooltip("controls the overall maniability of a kart.")]
    [Range(0f,1f)]
    public float maniability;

    [Tooltip("Changes the gravity but also the bumps when bumping against other karts.")]
    [Range(0f,1f)]
    public float weight;
    [Tooltip("Will help to get better items.")]
    [Range(0f,1f)]
    public float luck;

    // allow for stat adding for powerups.
    public static Stats operator +(Stats a, Stats b)
    {
        return new Stats
        {
            topSpeed     = a.topSpeed + b.topSpeed,
            acceleration = a.acceleration + b.acceleration,
            maniability  = a.maniability + b.maniability,
            weight       = a.weight + b.weight,
            luck         = a.luck + b.luck
        };
    }
    public static Stats operator *(Stats a, float c)
    {
        return new Stats
        {
            topSpeed     = a.topSpeed * c,
            acceleration = a.acceleration * c,
            maniability  = a.maniability * c,
            weight       = a.weight * c,
            luck         = a.luck * c
        };
    }
}
