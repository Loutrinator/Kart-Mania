using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDebugger : MonoBehaviour
{
    public float lerpSpeed = 5f;
    public float maxAngle = 40f;
    private PlayerAI ai;
    private Vector2 currentMovement;
    private Vector2 movements;
    [SerializeField] private Transform LJoystick;
    [SerializeField] private Transform RJoystick;
    [SerializeField] private Animator LT;
    [SerializeField] private Animator RT;
    [SerializeField] private MeshRenderer A;
    [SerializeField] private MeshRenderer B;
    [SerializeField] private MeshRenderer X;
    [SerializeField] private MeshRenderer Y;

    public void SetInputs(Vector2 moves)
    {
        movements = moves;
    }

    private void FixedUpdate()
    {
        
        currentMovement = Vector2.Lerp(currentMovement,movements,lerpSpeed * Time.fixedDeltaTime);
        LJoystick.localEulerAngles = new Vector3(maxAngle*currentMovement.x,0f,0f);
        //LT.SetBool("Pressed",currentMovement.y>0);
        //RT.SetBool("Pressed",currentMovement.y<0);
        int aPressed = currentMovement.y > 0 ? 1 : 0;
        int bPressed = currentMovement.y < 0 ? 1 : 0;
        A.material.SetInt("Pressed",aPressed);
        B.material.SetInt("Pressed",bPressed);

    }
}
