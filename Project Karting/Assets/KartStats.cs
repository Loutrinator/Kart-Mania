using UnityEngine;

[System.Serializable]
public struct KartStats
{
    [Header("Movement Settings")]
    [Tooltip("The maximum speed forwards")]
    public float topSpeed;

    [Tooltip("How quickly the Kart reaches top speed.")]
    public float acceleration;

    [Tooltip("The maximum speed backward.")]
    public float reverseSpeed;

    [Tooltip("The rate at which the kart increases its backward speed.")]
    public float reverseAcceleration;

    [Tooltip("How quickly the Kart starts accelerating from 0. A higher number means it accelerates faster sooner.")]
    [Range(0.2f, 1)]
    public float accelerationCurve;

    [Tooltip("How quickly the Kart slows down when going in the opposite direction.")]
    public float braking;

    [Tooltip("How quickly to slow down when neither acceleration or reverse is held.")]
    public float coastingDrag;

    [Range(0, 1)]
    [Tooltip("The amount of side-to-side friction.")]
    public float grip;

    [Tooltip("How quickly the Kart can turn left and right.")]
    public float steer;

    [Tooltip("Additional gravity for when the Kart is in the air.")]
    public float addedGravity;

    [Tooltip("How much the Kart tries to keep going forward when on bumpy terrain.")]
    [Range(0, 1)]
    public float suspension;

    // allow for stat adding for powerups.
    public static KartStats operator +(KartStats a, KartStats b)
    {
        return new KartStats
        {
            acceleration        = a.acceleration + b.acceleration,
            accelerationCurve   = a.accelerationCurve + b.accelerationCurve,
            braking             = a.braking + b.braking,
            coastingDrag        = a.coastingDrag + b.coastingDrag,
            addedGravity        = a.addedGravity + b.addedGravity,
            grip                = a.grip + b.grip,
            reverseAcceleration = a.reverseAcceleration + b.reverseAcceleration,
            reverseSpeed        = a.reverseSpeed + b.reverseSpeed,
            topSpeed            = a.topSpeed + b.topSpeed,
            steer               = a.steer + b.steer,
            suspension          = a.suspension + b.suspension
        };
    }
}
