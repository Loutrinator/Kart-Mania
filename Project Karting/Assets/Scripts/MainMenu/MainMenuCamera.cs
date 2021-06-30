using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public enum MainCameraPosition
    {screen, car, desk}

    public Animator cameraAnimator;
    private MainCameraPosition positionState;
    

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            cameraAnimator.SetTrigger("move");
        }
    }
}
