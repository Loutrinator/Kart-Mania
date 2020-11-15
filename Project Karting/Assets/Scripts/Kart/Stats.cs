using UnityEngine;

namespace Kart {
    /// <summary>
    /// Contains a series tunable parameters to tweak various karts for unique driving mechanics.
    /// </summary>
    [System.Serializable]
    public struct Stats
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

        [Tooltip("How quickly the Kart slows down when going in the opposite direction.")]
        public float braking;

        [Tooltip("How quickly the Kart can turn left and right.")]
        public float steer;

        [Tooltip("Additional gravity for when the Kart is in the air.")]
        public float addedGravity;

        [Tooltip("How much the Kart tries to keep going forward when on bumpy terrain.")]
        [Range(0, 1)]
        public float suspension;

        // allow for stat adding for powerups.
        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats
            {
                acceleration        = a.acceleration + b.acceleration,
                braking             = a.braking + b.braking,
                addedGravity        = a.addedGravity + b.addedGravity,
                reverseAcceleration = a.reverseAcceleration + b.reverseAcceleration,
                reverseSpeed        = a.reverseSpeed + b.reverseSpeed,
                topSpeed            = a.topSpeed + b.topSpeed,
                steer               = a.steer + b.steer,
                suspension          = a.suspension + b.suspension
            };
        }
    }
}