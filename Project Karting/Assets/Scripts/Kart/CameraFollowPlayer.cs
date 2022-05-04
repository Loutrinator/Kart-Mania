using System;
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
        public KartBase target;
        [SerializeField] private Transform frontPosition;
        [SerializeField] private Transform backPosition;
        [SerializeField] public Camera cam;
        public ShakeTransform cameraShakeTransform;

        private CameraMode currentCameraMode = CameraMode.front;
        private Vector3 desiredPosition;
        private Quaternion desiredRotation;

        private float kartDir = 0;

        private void Update()
        {
            kartDir = Mathf.Lerp(kartDir, target.GetDirection(), KartPhysicsSettings.instance.cameraKartDirectionLerp * Time.deltaTime);
        }

        private void LateUpdate()
        {
            Vector3 targetPos = target.transform.position;
            if(currentCameraMode == CameraMode.front) targetPos += transform.right * kartDir * KartPhysicsSettings.instance.cameraSideAmplitude;
            
            transform.position = Vector3.Lerp(transform.position, targetPos, KartPhysicsSettings.instance.cameraPositionLerp * Time.deltaTime);
            Quaternion targetRot = target.transform.rotation;
            targetRot *= Quaternion.Lerp(Quaternion.identity, Quaternion.AngleAxis(KartPhysicsSettings.instance.cameraSideAngleAmplitude * kartDir,target.transform.up),Mathf.Abs(this.kartDir)*2f);// new Quaternion(transform.up.x, transform.up.y, transform.up.z, KartPhysicsSettings.instance.cameraSideAngleAmplitude * kartDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, KartPhysicsSettings.instance.cameraRotationLerp * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(target.transform.position, transform.up*5f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(target.transform.position, target.transform.up*5f);
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
