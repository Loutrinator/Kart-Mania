using System.Collections.Generic;
using UnityEngine;

namespace Kart {
    public enum CameraMode
    {
        front,rear
    }
    public class CameraFollowPlayer : MonoBehaviour
    {
        public Transform target;
        public float rotationPercent = 0.1f;
        public Camera frontCamera;
        public Camera rearCamera;
        public ShakeTransform cameraShakeTransform;

        private CameraMode currentCameraMode = CameraMode.front;

        private void LateUpdate() {
            transform.position = target.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationPercent);
        }

        public void switchCameraMode(CameraMode mode)
        {
            if (currentCameraMode != mode)
            {
                currentCameraMode = mode;
                switch (mode)
                {
                    case CameraMode.front:
                        frontCamera.gameObject.SetActive(true);
                        rearCamera.gameObject.SetActive(false);
                        break;
                    case CameraMode.rear:
                        frontCamera.gameObject.SetActive(false);
                        rearCamera.gameObject.SetActive(true);
                        break;
                }
            }
        }
    }
}
