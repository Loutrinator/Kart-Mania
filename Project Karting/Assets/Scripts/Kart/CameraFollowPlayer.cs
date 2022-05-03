using System.Collections.Generic;
using Handlers;
using UnityEngine;

namespace Kart {
    public enum CameraMode
    {
        front,rear
    }
    public class CameraFollowPlayer : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private Transform frontPosition;
        [SerializeField] private Transform backPosition;
        [SerializeField] public Camera cam;
        public ShakeTransform cameraShakeTransform;

        private CameraMode currentCameraMode = CameraMode.front;
        private Vector3 desiredPosition;
        private Quaternion desiredRotation;

        private void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, KartPhysicsSettings.instance.cameraPositionLerp * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, KartPhysicsSettings.instance.cameraRotationLerp * Time.fixedDeltaTime);
        }

        public void SetViewport(int id)
        {
            int playerAmount = LevelManager.instance.gameConfig.players.Count;
            switch (playerAmount)
            {
                case 1 : return;
                case 2 :
                    switch (id)
                    {
                        case 0 :
                            cam.rect = new Rect(0, 0, 0.5f, 1);
                            break;
                        case 1 :
                            cam.rect = new Rect(0.5f, 0, 0.5f, 1);
                            break;
                    }
                    break;
                case 3 :
                    switch (id)
                    {
                        case 0 :
                            cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                            break;
                        case 1 :
                            cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                            break;
                        case 2 :
                            cam.rect = new Rect(0.5f, 0, 0.5f, 1);
                            break;
                    }
                    break;
                case 4 :
                    switch (id)
                    {
                        case 0 :
                            cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                            break;
                        case 1 :
                            cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                            break;
                        case 2 :
                            cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                            break;
                        case 3 :
                            cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                            break;
                    }
                    break;
            }
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
