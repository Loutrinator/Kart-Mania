using System;
using UnityEngine;

namespace Kart
{
    public class KartCollisions : MonoBehaviour
    {
        public KartBase kartBase;

        [SerializeField] private AudioClip clip;
        private AudioSource audio;
        private Vector3 collisionNormal = Vector3.zero;

        private void Start()
        {
            audio = gameObject.AddComponent<AudioSource>();
            audio.clip = clip;
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
                if(!audio.isPlaying) audio.Play();
                collisionNormal = other.contacts[0].normal;
                float forceLeftCoeff = 1 - KartPhysicsSettings.instance.borderVelocityLossPercent;
                kartBase.currentVelocity *= forceLeftCoeff;
                kartBase.currentForcesVelocity += collisionNormal * KartPhysicsSettings.instance.bumpForce;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            if(kartBase.transform != null)
                Gizmos.DrawRay(kartBase.transform.position, collisionNormal*3f);
            
        }
    }

}