using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plunger : MonoBehaviour
{
    private Rigidbody rb;
    public BoxCollider wood;
    public BoxCollider head;
    public Transform centerOfGravity;
    private Quaternion stuckRotation;

    private bool isStuck = false;
    private int nbCollisions = 0;
    public float lerpSpeed = 0.4f;
    public float speed = 30f;
    public float lifeTime = 10f;
    public int maxCollisions = 3;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfGravity.localPosition;
        rb.AddForce(-transform.forward * speed, ForceMode.Impulse);

    }

    // Update is called once per frame
    void Update()
    {
        if (!isStuck)
        {
            Ray ray = new Ray(transform.position, -transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 0.9f))
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
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, stuckRotation, lerpSpeed * Time.deltaTime);
            lifeTime -= Time.deltaTime;
            if (lifeTime < 0) Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<Plunger>())
        {
            Destroy(gameObject);
        }
        else
        {
            nbCollisions++;
            if(nbCollisions >= maxCollisions)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, -transform.forward * 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray);
    }
}
