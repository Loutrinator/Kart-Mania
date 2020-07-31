using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public float smoothedSpeed = 10.0f;
    public float lookAtOffset = 1f;
    public Vector3 dist;
    public KartController target;

    private void FixedUpdate()
    {
        Vector3 Pos = target.transform.position + target.transform.rotation * dist;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, Pos, smoothedSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPos;
        Debug.Log("target.roadNormal = " + target.gravityDirection);
        transform.LookAt(target.transform.position + target.gravityDirection*lookAtOffset);
        /*var forward = transform.forward;
        var Up = target.roadNormal;
        
        transform.rotation = Quaternion.LookRotation(Up.normalized, -forward.normalized);
        transform.Rotate(Vector3.right, 90f, Space.Self);*/
    }
}
