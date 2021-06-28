using UnityEditor.Rendering;
using UnityEngine;

namespace Player
{
    public class PlayerAction : IActions
    {
        public int Accelerate()
        {
            float accelerate = Input.GetAxis("Accelerate");
            float brake = Input.GetAxis("Brake");
            if (brake > 0.001f) return -1;
            if (accelerate > 0.001f) return 1;
            return 0;
        }

        public float Steer()
        {
            return Input.GetAxis("Horizontal");
        }

        public bool Drift()
        {
            return Input.GetAxis("Drift") > 0;
        }

        public bool ItemKeyHold()
        {
            return Input.GetButton("Item");
        }

        public bool ItemKeyDown()
        {
            return Input.GetButtonDown("Item");
        }

        public bool ItemKeyUp()
        {
            return Input.GetButtonUp("Item");
        }
        public bool ShowRearCamera()
        {
            return Input.GetButton("RearCamera");
        }
    }
}