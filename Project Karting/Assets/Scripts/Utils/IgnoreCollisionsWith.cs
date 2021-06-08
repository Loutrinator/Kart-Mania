using System;
using UnityEngine;

namespace Utils {
    public class IgnoreCollisionsWith : MonoBehaviour {
        public Collider selfCollider;
        public Collider[] ignoreColliders;

        private void Awake() {
            foreach (var ignoreCollider in ignoreColliders) {
                Debug.Log("Ignoring collisions");
                Physics.IgnoreCollision(selfCollider, ignoreCollider);
            }
        }

        private void OnCollisionEnter(Collision other) {
            Debug.Log(other.gameObject.name);
        }
    }
}
