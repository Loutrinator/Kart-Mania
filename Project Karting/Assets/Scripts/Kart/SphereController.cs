using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float forward = Input.GetAxis("Forward");
        rb.AddForce(transform.forward * (forward * speed));
    }
}
