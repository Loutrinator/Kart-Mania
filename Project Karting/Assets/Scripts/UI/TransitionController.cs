using System;
using UnityEngine;

public enum TransitionState {
    Hidden, FadeIn, BlackedOut, FadeOut
}
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

    private Action _onShowLoading;
    private Action _onHideLoading;

    public static TransitionController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        }

        Instance = this;
    }

    private void Update()
    {
        float elapsed;
        float animationPercent;
        float xPos;

        if (startTransition)
        {
            startTransition = false;
            if (transitionState == TransitionState.Hidden)
            {
                FadeIn();
            } else if (transitionState == TransitionState.BlackedOut)
            {
                FadeOut();
            }
        }
        
        switch (transitionState)
        {
            case TransitionState.Hidden:
                break;
            case TransitionState.BlackedOut:
                break;
            case TransitionState.FadeIn:
                elapsed = Time.time - currentStateStartTime;
                animationPercent = elapsed / transitionDuration;

                xPos = fadeInCurve.Evaluate(animationPercent);
                transform.localPosition = new Vector3(xPos * -XPosition, 0, 0);

                if (elapsed >= transitionDuration)
                {
                    currentStateStartTime = Time.time;
                    transitionState = TransitionState.BlackedOut;
                    ShowLoading();
                }

                break;
            case TransitionState.FadeOut:
                elapsed = Time.time - currentStateStartTime;
                animationPercent = elapsed / transitionDuration;

                xPos = fadeOutCurve.Evaluate(animationPercent);
                transform.localPosition = new Vector3(xPos * XPosition, 0, 0);

                if (elapsed >= transitionDuration)
                {
                    currentStateStartTime = Time.time;
                    transitionState = TransitionState.Hidden;
                }

                break;
        }
    }
    
    public void FadeIn(Action onLoadingStart = null)    // transition start
    {
        currentStateStartTime = Time.time;
        transitionState = TransitionState.FadeIn;
        _onShowLoading = onLoadingStart;
        SoundManager.Instance.FadeOutMusic();
    }
    public void FadeOut(Action onLoadingEnd = null)    // transition end
    {
        currentStateStartTime = Time.time;
        transitionState = TransitionState.FadeOut;
        _onHideLoading = onLoadingEnd;
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
        if (_onShowLoading != null) {
            _onShowLoading.Invoke();
            _onShowLoading = null;
        }
    }
    public void HideLoading()
    {
        loadingImage.SetBool("visible", false);
        if (_onHideLoading != null) {
            _onHideLoading.Invoke();
            _onHideLoading = null;
        }
    }
}
