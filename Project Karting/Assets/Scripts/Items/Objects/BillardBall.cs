using Road.RoadPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillardBall : PhysicsObject
{
    public float coeff;
    public float lifetime = 20.0f;
    public override bool IsGrounded()
    {
        return false;
    }

    void Awake()
    {

    }

    void FixedUpdate()
    {
        if (this.enabled)
        {
            rigidBody.AddForce(-closestBezierPos.Tangent * coeff, ForceMode.Force);
            lifetime -= Time.deltaTime;

            if (lifetime < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
