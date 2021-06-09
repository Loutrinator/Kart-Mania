using UnityEngine;

namespace Kart
{
    public class KartCollisions : MonoBehaviour
    {
        public KartBase kartBase;
        public float bumpForce = 20f;

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
                Vector3 collisionNormal = other.contacts[0].normal;
                kartBase.currentForcesVelocity += collisionNormal * bumpForce;
            }
        }
    }

}