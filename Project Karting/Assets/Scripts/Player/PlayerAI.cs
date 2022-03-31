using System.Collections;
using System.Collections.Generic;
using Handlers;
using System.Linq;
using Kart;
using UnityEditor;
using UnityEngine;

using AI;
using AI.UtilityAI;
using Player;

public class PlayerAI : KartController
{
    public AIController aiController;

    public Vector2 movement;
    
    private void Start()
    {
        if (kart == null)
            kart = GetComponent<KartBase>();
        
        if (aiController == null) 
            aiController = gameObject.AddComponent<UtilityAIController>();
        
        //aiController.kart = kart;
        movement = Vector2.zero;
        AIManager.Instance.playersAiUpdate.Add(AIUpdate);
    }
    
    private void AIUpdate()
    {
        
        List<AIAction> bestActions = aiController.tick();
        string actionsChosenDesc = "";
        foreach (var action in bestActions)
        {
            actionsChosenDesc += action.actionName + " ";
            switch (action.actionName)
            {
                case "Accelerate":
                    movement.y = 1;
                    break;
                case "Forward":
                    movement.x = 0;
                    break;
                case "Brake":
                    movement.y = -1;
                    break;
                case "TurnLeft":
                    movement.x = -1;
                    break;
                case "TurnRight":
                    movement.x = 1;
                    break;
                case "DriftOn":
                    Drift(true);
                    break;
                case "DriftOff":
                    Drift(false);
                    break;
                case "LetGo":
                    movement.y = 0;
                    break;
            }
        }
        Debug.Log(actionsChosenDesc);
        Move(movement);
    }

    /*
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
        if (ctx.action.name == _controls.Kart.Pause.name)
        {
            OnPause(ctx);
        }
    }
    
    
    public void OnMove(InputAction.CallbackContext context)
    {
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
        if (_kart != null)
        {
            _kart.drift = context.ReadValueAsButton();
        }
    }
    public void OnRearCamera(InputAction.CallbackContext context)
    {
        if (_kart != null)
        {
            _kart.rear = context.ReadValue<float>() > 0 ;
        }
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        GameManager.Instance.Pause();
    }
        */
}
