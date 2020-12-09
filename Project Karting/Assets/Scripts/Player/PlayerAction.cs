using UnityEngine;

namespace Player
{
    public class PlayerAction : IActions
    {
        public int Accelerate()
        {
            float forwardInput = Input.GetAxis("Vertical");
            if (forwardInput > 0.001f) return 1;
            if (forwardInput < -0.001f) return -1;
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
            var a =  Input.GetButtonDown("Item");
            return Input.GetButtonDown("Item");
        }

        public bool ItemKeyUp()
        {
            return Input.GetButtonUp("Item");
        }
    }
}