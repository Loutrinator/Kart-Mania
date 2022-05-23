using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class MarbleLauncher : MonoBehaviour
{
    [SerializeField] private LineRenderer leftRubber;
    [SerializeField] private LineRenderer rightRubber;
    [SerializeField] private List<Transform> leftRubberAttachment;
    [SerializeField] private List<Transform> rightRubberAttachment;
    [SerializeField] private Transform marbleHolder;
    [SerializeField] private float releaseSpeed = 0.25f;
    [SerializeField] private float stretchSpeed = 2f;
    [SerializeField] private float stretchMaxDistance = 5f;

    private Vector3 restPosition;

    private void Start()
    {
        restPosition = marbleHolder.localPosition;
        leftRubber.positionCount = leftRubberAttachment.Count;
        rightRubber.positionCount = rightRubberAttachment.Count;
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
        marbleHolder.DOLocalMove(restPosition,releaseSpeed).SetEase(Ease.OutElastic);
    }
}
