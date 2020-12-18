using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Kart;
public class CameraFollowPlayer : MonoBehaviour
{
    public float lerpSpeed = 10.0f;
    public float yRotation = 2f;
    public float xOffset = 2f;
    //public float rotationLerpCoeff;
    //public float lookAtOffset = 1f;//3.65
    //public Vector3 dist;// 0 4.31 -7.1
    public KartBase target;

    private float lerpedYRot;
    private float lerpedX;
    private Vector3 startPos;
    private Quaternion startRot;
    public float lookAtHeight;

    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        lookAtHeight = startPos.y;
    }

    private void LateUpdate()
    {
        float desiredYRot = target.GetHorizontalAxis() * yRotation;
        lerpedYRot = Mathf.Lerp(lerpedYRot, desiredYRot, Time.fixedDeltaTime * lerpSpeed);
        float desiredX = target.GetHorizontalAxis() * xOffset;
        lerpedX = Mathf.Lerp(lerpedYRot, desiredX, Time.fixedDeltaTime * lerpSpeed);
        transform.localPosition = startPos;
        Vector3 previousPos = transform.localPosition;
        transform.localPosition = new Vector3(lerpedX,previousPos.y,previousPos.z);
        transform.localRotation = startRot;
        transform.RotateAround(target.transform.position + Vector3.up*lookAtHeight,Vector3.up, lerpedYRot);
        //transform.LookAt(target.transform.position + Vector3.up*lookAtHeight);
        /*
        Transform kartTransform = target.transform;
        Transform cameraTransform = transform;
        Vector3 kartPosition = kartTransform.position;
        Vector3 pos = kartPosition + kartTransform.rotation * dist;
        Vector3 smoothedPos = Vector3.Lerp(cameraTransform.position, pos, smoothedSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPos;
        //Debug.Log("target.roadNormal = " + target.GetRoadDirection());
        transform.LookAt(kartPosition + target.getRoadDirection()*lookAtOffset);
        //var forward = cameraTransform.forward;
        //var up = target.getRoadDirection();
        //Quaternion oldRotation = cameraTransform.rotation;
        //cameraTransform.rotation = Quaternion.LookRotation(up.normalized, -forward.normalized);
        //cameraTransform.Rotate(Vector3.right, 90f, Space.Self);
        //cameraTransform.rotation = Quaternion.Lerp(oldRotation,cameraTransform.rotation,rotationLerpCoeff);*/
    }
    /*
    private void OnDrawGizmos()
    {
        Transform cameraTransform = transform;
        Vector3 cameraPosition = cameraTransform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(cameraPosition, cameraTransform.up);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPosition, target.getRoadDirection());
    }*/

}
