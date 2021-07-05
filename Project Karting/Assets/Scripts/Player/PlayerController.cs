using Handlers;
using System.Linq;
using Kart;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private KartBase _kart;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            linkToKart();
        }
        /*public void Update()
        {
            //TODO : inputs kart selection menu
            Vector2 movement = _actionsOutputs.Movement();
            _info.kart.forwardMove = movement[0];
            _info.kart.hMove = movement[1];
            _info.kart.drift = _actionsOutputs.Drift();
            if (_info.hasItem) {
                if (_actionsOutputs.ItemKeyHold()) _info.Item.OnKeyHold(_info);
                if (_actionsOutputs.ItemKeyDown()) _info.Item.OnKeyDown(_info);
                if (_actionsOutputs.ItemKeyUp()) _info.Item.OnKeyUp(_info);
            }

            if (_actionsOutputs.ShowRearCamera())
            {
                _info.camera.switchCameraMode(CameraMode.rear);
            }
            else
            {
                _info.camera.switchCameraMode(CameraMode.front);
            }
        }*/

        private void linkToKart()
        {
            var karts = FindObjectsOfType<KartBase>();
            var index = _playerInput.playerIndex;
            _kart = karts.FirstOrDefault(k => k.playerIndex == index);
        }


        public void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log("MOVE");
            if (_kart != null)
            {
                Vector2 movement = context.ReadValue<Vector2>();
                float x = movement[0] > 0.1f ? 1f : movement[0] < -0.1f ? -1f : 0;
                float y = movement[1] > 0.1f ? 1f : movement[1] < -0.1f ? -1f : 0;
                _kart.movement = new Vector2(x,y);
            }
        }

        public void OnDrift(InputAction.CallbackContext context)
        {
            Debug.Log("DRIFT");
            if (_kart != null)
            {
                _kart.drift = context.ReadValueAsButton();
            }
        }
        public void OnRearCamera(InputAction.CallbackContext context)
        {
            Debug.Log("REAR");
            if (_kart != null)
            {
                //_kart.rear = context.ReadValue<bool>();
            }
        }
        
    }
}