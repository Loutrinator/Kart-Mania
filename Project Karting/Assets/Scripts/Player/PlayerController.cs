using System;
using Handlers;
using System.Linq;
using System.Runtime.CompilerServices;
using Kart;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : KartController
    {
        private PlayerConfiguration playerConfig;
        [SerializeField] private PlayerInput input;
        private PlayerControls _controls;
        private void Awake()
        {
            var kart = GetComponent<KartBase>();
            _controls = new PlayerControls();
        }

        public void InitializePlayerConfiguration(PlayerConfiguration pc)
        {
            playerConfig = pc;
            playerConfig.Input.onActionTriggered += Input_OnActionTriggered;
            input = playerConfig.Input;
        }

        private void Input_OnActionTriggered(InputAction.CallbackContext ctx)
        {
            if (ctx.action.name == _controls.Kart.Movement.name)
            {
                OnMove(ctx);
            }
            if (ctx.action.name == _controls.Kart.Drift.name)
            {
                OnDrift(ctx);
            }
            if (ctx.action.name == _controls.Kart.Rearcamera.name)
            {
                OnRearCamera(ctx);
            }
            if (ctx.action.name == _controls.Kart.Pause.name || ctx.action.name == _controls.UI.Back.name)
            {
                OnPause(ctx);
            }
        }
        
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Move(context.ReadValue<Vector2>());
        }

        public void OnDrift(InputAction.CallbackContext context)
        {
            Drift(context.ReadValueAsButton());
        }
        public void OnRearCamera(InputAction.CallbackContext context)
        {
            if (kart != null)
            {
                kart.rear = context.ReadValue<float>() > 0 ;
            }
        }
        public void OnPause(InputAction.CallbackContext context)
        {
            if(GameManager.Instance != null)
                GameManager.Instance.Pause(playerConfig);
        }
        
    }
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