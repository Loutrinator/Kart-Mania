using Road.RoadPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillardBall : PhysicsObject
{
    public AudioSource audioSource;
    public AudioClip spawn;
    [SerializeField] private AudioClip pop;
    [SerializeField] private AudioClip roll;
    [SerializeField] private AudioClip bounce;

    public float coeff;
    public float lifetime = 20.0f;

    public bool isThrown = false;
    public float time;
    public override bool IsGrounded()
    {
        return false;
    }

    void Awake()
    {
        time = pop.length;
        rigidBody.isKinematic = true;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void FixedUpdate()
    {
        if (isThrown)
        {
            rigidBody.AddForce(-closestBezierPos.Tangent * coeff, ForceMode.Force);
            lifetime -= Time.deltaTime;
            if (!audioSource.isPlaying)
            
            {
                audioSource.clip = roll;
                audioSource.Play();
                audioSource.loop = true;
            }

            if (lifetime < 0)
            {
                if (!audioSource.clip != pop) {
                    audioSource.loop = false;
                    audioSource.clip = pop;
                    if(!audioSource.isPlaying) audioSource.Play();
                }
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    isThrown = false;
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10) return;
        audioSource.loop = false;
        audioSource.clip = bounce;
        audioSource.Play();
    }
}
