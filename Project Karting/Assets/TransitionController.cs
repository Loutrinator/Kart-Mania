using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionController : MonoBehaviour
{
    public Image image;
    public bool openByDefault;
    public bool startTransition;
    private bool isTransitioning;
    public float transitionSpeed;
    private float completion;
    private int direction;

    private void Start()
    {
        if (openByDefault)
        {
            completion = 0;
        }
        else
        {
            completion = 1;
        }
    }

    private void Update()
    {
        if (startTransition)
        {
            StartTransition();
        }

        if (isTransitioning)
        {
            completion += direction * transitionSpeed * Time.deltaTime;
            image.material.SetFloat("Cutoff",completion);
            if ((direction == 1 && completion >= 1) || (direction == -1 && completion <= 0))
            {
                isTransitioning = false;
            }
        }
        
        
    }

    private void StartTransition()
    {
        startTransition = false;
        isTransitioning = true;
        if (completion >= 1)
        {
            completion = 0;
            direction = 1;
        }else if(completion <= 0)
        {
            completion = 1;
            direction = -1;
        }
    }
}
