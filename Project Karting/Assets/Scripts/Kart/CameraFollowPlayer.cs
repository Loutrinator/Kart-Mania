using System.Collections.Generic;
using UnityEngine;

namespace Kart {
    public class CameraFollowPlayer : MonoBehaviour
    {
        public Transform target;
        public float rotationPercent = 0.1f;
        public Camera frontCamera;
        public Camera rearCamera;

 
        private void LateUpdate() {
            transform.position = target.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationPercent);
        }
    }
}
