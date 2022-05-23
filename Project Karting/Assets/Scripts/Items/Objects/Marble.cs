using System.Collections;
using UnityEngine;

namespace Items
{
    public class Marble : MonoBehaviour
    {
        [SerializeField] private MeshRenderer insideRenderer;
        [SerializeField] private Rigidbody rb;
        public void SetColor(Color color)
        {
            insideRenderer.material.SetColor("MainColor", color);
        }

        public void Freeze()
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        public void Shoot(Vector3 direction, float intensity)
        {
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
    }
}