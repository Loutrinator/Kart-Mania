using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraFollowPlayer : MonoBehaviour
{
    public float smoothedSpeed = 10.0f;
    [Range(0,1)]
    public float rotationLerpCoeff;
    public float lookAtOffset = 1f;
    public Vector3 dist;
    public KartController target;

    private void Update()
    {
        Transform kartTransform = target.transform;
        Transform cameraTransform = transform;
        Vector3 kartPosition = kartTransform.position;
        Vector3 pos = kartPosition + kartTransform.rotation * dist;
        Vector3 smoothedPos = Vector3.Lerp(cameraTransform.position, pos, smoothedSpeed * Time.deltaTime);
        transform.position = smoothedPos;
        Debug.Log("target.roadNormal = " + target.gravityDirection);
        transform.LookAt(kartPosition + target.gravityDirection*lookAtOffset);
        var forward = cameraTransform.forward;
        var up = target.roadNormal;
        Quaternion oldRotation = cameraTransform.rotation;
        cameraTransform.rotation = Quaternion.LookRotation(up.normalized, -forward.normalized);
        cameraTransform.Rotate(Vector3.right, 90f, Space.Self);
        cameraTransform.rotation = Quaternion.Lerp(oldRotation,cameraTransform.rotation,rotationLerpCoeff);
    }

    private void OnDrawGizmos()
    {
        Transform cameraTransform = transform;
        Vector3 cameraPosition = cameraTransform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(cameraPosition, cameraTransform.up);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPosition, target.roadNormal);
    }
}
