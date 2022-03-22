using System.Collections.Generic;
using Handlers;
using UnityEngine;

namespace Kart {
    public class AICamera : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private Transform frontPosition;
        [SerializeField] private Transform backPosition;
        [SerializeField] private float rotationPercent = 0.1f;
        [SerializeField] public Camera cam;
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
                        cam.gameObject.transform.position = frontPosition.position;
                        cam.gameObject.transform.rotation = frontPosition.rotation;
                        break;
                    case CameraMode.rear:
                        cam.gameObject.transform.position = backPosition.position;
                        cam.gameObject.transform.rotation = backPosition.rotation;
                        break;
                }
            }
        }
    }
}
