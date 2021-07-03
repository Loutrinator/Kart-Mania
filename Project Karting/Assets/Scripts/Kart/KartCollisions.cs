using UnityEngine;

namespace Kart
{
    public class KartCollisions : MonoBehaviour
    {
        public KartBase kartBase;
        public float bumpForce = 50f;

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
                Vector3 collisionNormal = other.contacts[0].normal;
                float forceLeftCoeff = 1 - KartPhysicsSettings.instance.borderVelocityLossPercent;
                kartBase.rigidBody.velocity *= forceLeftCoeff;
                kartBase.currentForcesVelocity *= forceLeftCoeff;
                kartBase.currentForcesVelocity += collisionNormal * bumpForce;
            }
        }
    }

}