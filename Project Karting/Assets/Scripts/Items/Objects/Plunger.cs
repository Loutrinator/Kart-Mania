using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class Plunger : ItemObject
    {
        private Rigidbody rb;
        public BoxCollider wood;
        public BoxCollider head;
        public Transform centerOfGravity;
        private Quaternion stuckRotation;

        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip soundThrown;
        [SerializeField] private AudioClip soundStick;
        [SerializeField] private AudioClip soundBump;

        private bool isStuck = false;
        private bool isThrown = false;
        private int nbCollisions = 0;
        public float lerpSpeed = 0.4f;
        public float speed = 30f;
        public float lifeTime = 10f;
        public int maxCollisions = 3;

        public float offset = 2.0f;
        public float immunity = 0.2f;

        public float timeShooted;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.centerOfMass = centerOfGravity.localPosition;

        }

        // Update is called once per frame
        void Update()
        {
            if (isThrown)
            {
                if (!isStuck && (Time.time - timeShooted) >= immunity)
                {
                    Ray ray = new Ray(transform.position, -transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 0.9f))
                    {
                        if (!(hit.transform.name.Contains("Checkpoint")))
                        {
                            //Debug.Log(hit.transform.name);

                            rb.velocity = Vector3.zero;
                            rb.isKinematic = true;
                            transform.SetParent(hit.transform, true);

                            rb.useGravity = false;

                            Vector3 newForward = hit.normal;

                            Vector3 newUp = transform.up;

                            Vector3 newRight = Vector3.Cross(newUp, newForward);

                            newUp = Vector3.Cross(newForward, newRight);

                            transform.forward = newForward;
                            transform.up = newUp;
                            transform.right = newRight;
                            stuckRotation = Quaternion.LookRotation(newForward, newUp);
                            isStuck = true;
                            source.clip = soundStick;
                            source.Play();
                            source.clip = soundBump;
                            source.spatialBlend = 0;
                        }
                    }
                }
                else
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, stuckRotation, lerpSpeed * Time.deltaTime);
                    lifeTime -= Time.deltaTime;
                    if (lifeTime < 0) Destroy(gameObject);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.GetComponent<Kart.KartBase>() && isStuck)
            {
                if (collision.transform == transform.parent) return;
                Debug.Log("collision : " + collision.transform.name);
                Vector3 dir = collision.transform.position - transform.position;
                Debug.DrawRay(transform.position, dir, Color.red, 2.0f);
                collision.transform.GetComponent<Kart.KartBase>().currentForcesVelocity += dir * 1000;
                source.Play();
            }
        }

        private void OnDrawGizmos()
        {
            Ray ray = new Ray(transform.position, -transform.forward * 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray);
        }

        public override void ResetItem()
        {
            isStuck = false;
            isThrown = false;
            lifeTime = 10.0f;
        }

        public override void OnKeyHold(PlayerRaceInfo info)
        {
            return;
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            Use(info);
            isThrown = true;
            transform.parent = null;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(-transform.forward * speed + info.kart.currentVelocity, ForceMode.Impulse);
            timeShooted = Time.time;
            source.clip = soundThrown;
            source.Play();
        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {
            return;
        }
    }
}