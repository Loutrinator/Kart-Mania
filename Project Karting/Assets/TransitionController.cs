using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TransitionState{hidden,fadeIn,blackedOut,fadeOut}
public class TransitionController : MonoBehaviour
{
    public bool startTransition;
    public Animator loadingImage;
    public TransitionState transitionState;
    public float XPosition;
    public AnimationCurve fadeInCurve;
    public AnimationCurve fadeOutCurve;
    public float transitionDuration;
    
    private bool isTransitioning;
    private float completion;

    private float currentStateStartTime;
    

    private void Update()
    {
        float elapsed = 0f;
        float animationPercent = 0f;
        float xPos = 0f;

        if (startTransition)
        {
            startTransition = false;
            if (transitionState == TransitionState.hidden)
            {
                FadeIn();
            }else if (transitionState == TransitionState.blackedOut)
            {
                FadeOut();
            }
        }
        
        switch (transitionState)
        {
            case TransitionState.hidden:
                break;
            case TransitionState.blackedOut:
                break;
            case TransitionState.fadeIn:
                elapsed = 0f;
                elapsed = Time.time - currentStateStartTime;
                animationPercent = elapsed / transitionDuration;

                xPos = fadeInCurve.Evaluate(animationPercent);
                transform.localPosition = new Vector3(xPos * -XPosition, 0, 0);

                if (elapsed >= transitionDuration)
                {
                    currentStateStartTime = Time.time;
                    transitionState = TransitionState.blackedOut;
                    ShowLoading();
                }

                break;
            case TransitionState.fadeOut:
                elapsed = Time.time - currentStateStartTime;
                animationPercent = elapsed / transitionDuration;

                xPos = fadeOutCurve.Evaluate(animationPercent);
                transform.localPosition = new Vector3(xPos * XPosition, 0, 0);

                if (elapsed >= transitionDuration)
                {
                    currentStateStartTime = Time.time;
                    transitionState = TransitionState.hidden;
                }

                break;
        }
    }
    
    public void FadeIn()
    {
        currentStateStartTime = Time.time;
        transitionState = TransitionState.fadeIn;
    }
    public void FadeOut()
    {
        currentStateStartTime = Time.time;
        transitionState = TransitionState.fadeOut;
        HideLoading();
    }

    public void ResetToHide()
    {
        
    }

    public void ResetToClear()
    {
        
    }
    
    public void ShowLoading()
    {
        loadingImage.SetBool("visible", true);
    }
    public void HideLoading()
    {
        loadingImage.SetBool("visible", false);
    }
}
