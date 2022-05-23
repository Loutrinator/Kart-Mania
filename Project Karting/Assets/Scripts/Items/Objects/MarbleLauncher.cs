using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using Items;

public class MarbleLauncher : ItemObject
{
    [SerializeField] private LineRenderer leftRubber;
    [SerializeField] private LineRenderer rightRubber;
    [SerializeField] private List<Transform> leftRubberAttachment;
    [SerializeField] private List<Transform> rightRubberAttachment;
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Transform marbleHolder;
    [SerializeField] private Transform marble;
    [SerializeField] private float releaseSpeed = 0.25f;
    [SerializeField] private float stretchSpeed = 2f;
    [SerializeField] private float stretchMaxDistance = 5f;
    [SerializeField] private int maxMarbles = 3;

    private Vector3 restPosition;
    private int remainingMarbles = 3;

    private void Start()
    {
        restPosition = marbleHolder.localPosition;
        leftRubber.positionCount = leftRubberAttachment.Count;
        rightRubber.positionCount = rightRubberAttachment.Count;
        ResetItem();
    }

    private void Update()
    {
        leftRubber.SetPosition(0,leftRubberAttachment[0].position);
        leftRubber.SetPosition(1,leftRubberAttachment[1].position);
        rightRubber.SetPosition(0,rightRubberAttachment[0].position);
        rightRubber.SetPosition(1,rightRubberAttachment[1].position);
    }

    public void StretchRubber()
    {
        Debug.Log("MarbleLauncher : StretchRubber");
        marbleHolder.DOKill();
        marbleHolder.DOLocalMove(restPosition - Vector3.forward * stretchMaxDistance,stretchSpeed).SetEase(Ease.OutCubic);
    }
    public void ShootMarble()
    {
        Debug.Log("MarbleLauncher : ShootMarble");
        marbleHolder.DOKill();
        marbleHolder.DOLocalMove(restPosition, releaseSpeed).SetEase(Ease.OutElastic);
        remainingMarbles--;
        
        StartCoroutine(waitToRelease());
    }
    private IEnumerator waitToRelease()
    {
        //0.128 is the time at wich in DOTween's EaseOutElastic reaches its peak value
        yield return new WaitForSeconds(0.128f * releaseSpeed);
        //Destroy(marble.gameObject);
        if (remainingMarbles <= 0)
        {
            ResetItem();
        }
    }

    public override void ResetItem()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }

        remainingMarbles = maxMarbles;
        inUse = false;
    }

    private void ShowItem()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    public override void OnKeyHold(PlayerRaceInfo info)
    {
        Debug.Log("MarbleLauncher : OnKeyHold");
    }

    public override void OnKeyDown(PlayerRaceInfo info)
    {
        Debug.Log("MarbleLauncher : OnKeyDown");
        Transform parent = info.kart.transform;
        if (!inUse)
        {
            inUse = true;
            ShowItem();
        }
        StretchRubber();
    }

    public override void OnKeyUp(PlayerRaceInfo info)
    {
        Debug.Log("MarbleLauncher : OnKeyUp");
        ShootMarble();
    }
}
