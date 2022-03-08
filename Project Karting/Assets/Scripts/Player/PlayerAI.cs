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
    [SerializeField] private AIController _ai;
    [SerializeField] private KartBase kart;

    private Vector2 movement;
    
    private void Awake()
    {
        if (kart == null)
            kart = GetComponent<KartBase>();
        
        if (_ai == null) 
            _ai = new UtilityAIController();
        
        _ai.kart = kart;
        movement = Vector2.zero;
        
        GameManager.Instance.playersAiUpdate.Add(AIUpdate);
    }

    private void AIUpdate()
    {
        AIAction bestAction = _ai.tick();

        if (bestAction.actionName == "Accelerate")
        {
            movement.y = 1;
            Move(movement);
        }
        else if(bestAction.actionName == "Brake")
        {
            
            movement.y = 0;
            Move(movement);
        }
        else if(bestAction.actionName == "TurnLeft")
        {
            
            movement.x = -1;
            Move(movement);
        }
        else if(bestAction.actionName == "TurnRight")
        {
            movement.x = 1;
            Move(movement);
        }
        
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
