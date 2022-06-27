using Road.RoadPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillardBall : PhysicsObject
{
    public float coeff;
    public float lifetime = 20.0f;

    public bool isThrown = false;
    public override bool IsGrounded()
    {
        return false;
    }

    void Awake()
    {
        rigidBody.isKinematic = true;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }

    void FixedUpdate()
    {
        if (isThrown)
        {
            rigidBody.AddForce(-closestBezierPos.Tangent * coeff, ForceMode.Force);
            lifetime -= Time.deltaTime;

            if (lifetime < 0)
            {
                isThrown = false;
                Destroy(gameObject);
            }
        }
    }
}
