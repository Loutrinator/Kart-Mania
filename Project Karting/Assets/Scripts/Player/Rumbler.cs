using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RumblePattern
{
    None, Constant, Pulse, Linear, Hold
}

public class Rumbler : MonoBehaviour
{
    private PlayerInput _playerInput;
    public RumblePattern activeRumblePattern;
    private float _low;
    private float _high;
    private float _rumbleDuration;
    private float pulseDuration;
    private float rumbleStep;
    private bool isMotorActive;

    public void RumbleConstant(float low, float high, float duration)
    {
        activeRumblePattern = RumblePattern.Constant;
        _low = low;
        _high = high;
        _rumbleDuration = Time.time + duration;
        Invoke(nameof(StopRumble), duration);
    }
    
    public void RumblePulse(float low, float high, float duration,float burstLength)
    {
        activeRumblePattern = RumblePattern.Pulse;
        _low = low;
        _high = high;
        pulseDuration = Time.time + burstLength;
        _rumbleDuration = Time.time + duration;
        isMotorActive = true;
        var g = GetGamepad();
        g?.SetMotorSpeeds(low,high);
        rumbleStep = burstLength;
        Invoke(nameof(StopRumble), duration);
    }
    public void RumbleHold(float low, float high)
    {
        activeRumblePattern = RumblePattern.Hold;
        _low = low;
        _high = high;
        isMotorActive = true;
        var g = GetGamepad();
        g?.SetMotorSpeeds(low,high);
    }

    public void StopRumble()
    {
        var gamepad = GetGamepad();
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }

        activeRumblePattern = RumblePattern.None;
        isMotorActive = false;
    }

    public void SetPlayerInput(PlayerInput p)
    {
        _playerInput = p;
    }
    private void Update()
    {
        if (Time.time > _rumbleDuration && activeRumblePattern != RumblePattern.Hold) return;
        var gamepad = GetGamepad();
        if (gamepad == null) return;

        switch (activeRumblePattern)
        {
            case RumblePattern.Constant:
                gamepad.SetMotorSpeeds(_low,_high);
                break;
            case RumblePattern.Linear:
                break;
            case RumblePattern.Pulse:
                if (Time.time > pulseDuration)
                {
                    isMotorActive = !isMotorActive;
                    pulseDuration = Time.time + rumbleStep;
                    if (!isMotorActive)
                    {
                        gamepad.SetMotorSpeeds(0,0);
                    }
                    else
                    {
                        gamepad.SetMotorSpeeds(_low,_high);
                    }
                }
                break;
            case RumblePattern.Hold:
                if (isMotorActive)
                {
                    gamepad.SetMotorSpeeds(_low,_high);
                }
                else
                {
                    gamepad.SetMotorSpeeds(0,0);
                }
                break;
            case RumblePattern.None:
                return;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        StopRumble();
    }

    private Gamepad GetGamepad()
    {
        return Gamepad.all.FirstOrDefault(g => _playerInput.devices.Any(d => d.deviceId == g.deviceId));
    }
}
