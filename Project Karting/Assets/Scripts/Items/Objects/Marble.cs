using System;
using System.Collections;
using Kart;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items
{
    public class Marble : MonoBehaviour
    {
        [SerializeField] private GameObject marbleExplosionPrefab;
        [SerializeField] private float minIntensityForShockwave = 3f;
        [SerializeField] private MeshRenderer insideRenderer;
        [SerializeField] private Rigidbody rb;
        private GameObject marbleExplosion;
        private bool shockwaveActivated = false;

        public void LateUpdate()
        {
            if (shockwaveActivated && marbleExplosion != null)
            {
                marbleExplosion.transform.localRotation = Quaternion.LookRotation(Vector3.right,-rb.velocity);
            }
        }

        public void SetColor(Color color)
        {
            insideRenderer.material.SetColor("MainColor", color);
        }
        public void SetRandomColor()
        {
            insideRenderer.material.SetColor("MainColor", Random.ColorHSV(0f, 1f, 0f, 0.9f, 1f, 1f));
        }

        public void Freeze()
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        public void Shoot(Vector3 direction, float intensity)
        {
            if (intensity > minIntensityForShockwave)
            {
                marbleExplosion = Instantiate(marbleExplosionPrefab, transform.position, transform.rotation,transform);
                shockwaveActivated = true;
            }
            
            transform.parent = null;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(direction*intensity,ForceMode.Impulse);
            StartCoroutine(WaitForDissapear());
        }

        private IEnumerator WaitForDissapear()
        {
            yield return new WaitForSeconds(10f);
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            
            Destroy(marbleExplosion.gameObject);
            KartBase k = collision.collider.GetComponentInParent<KartBase>();
            if (k != null)
            {
                k.Damaged();
                Destroy(gameObject);
            }
        }
    }
}