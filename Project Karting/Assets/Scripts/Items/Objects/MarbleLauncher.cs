using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using Items;
using Random = UnityEngine.Random;

public class MarbleLauncher : ItemObject
{
    [SerializeField] private Marble marblePrefab;
    [SerializeField] private LineRenderer leftRubber;
    [SerializeField] private LineRenderer rightRubber;
    [SerializeField] private List<Transform> leftRubberAttachment;
    [SerializeField] private List<Transform> rightRubberAttachment;
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Transform marbleHolder;
    [SerializeField] private float releaseSpeed = 0.25f;
    [SerializeField] private float releaseTiming = 0.10f;
    [SerializeField] private float throwIntensity = 0.01f;
    [SerializeField] private float stretchSpeed = 2f;
    [SerializeField] private float stretchMaxDistance = 5f;
    [SerializeField] private int maxMarbles = 3;
    

    private Vector3 restPosition;
    private int remainingMarbles = 3;
    private Marble currentMarble;
    private bool marbleLocked;

    private void Start()
    {
        restPosition = marbleHolder.localPosition;
        leftRubber.positionCount = leftRubberAttachment.Count;
        rightRubber.positionCount = rightRubberAttachment.Count;
        ResetItem();
    }

    private void FixedUpdate()
    {
        leftRubber.SetPosition(0,leftRubberAttachment[0].position);
        leftRubber.SetPosition(1,leftRubberAttachment[1].position);
        rightRubber.SetPosition(0,rightRubberAttachment[0].position);
        rightRubber.SetPosition(1,rightRubberAttachment[1].position);
    }

    private void LateUpdate()
    {
        if(marbleLocked)
        {
            currentMarble.transform.position = marbleHolder.position;
        }
    }

    public void StretchRubber()
    {
        marbleHolder.DOKill();
        marbleHolder.DOLocalMove(restPosition - Vector3.forward * stretchMaxDistance,stretchSpeed).SetEase(Ease.OutCubic);
        if (marbleLocked == false && remainingMarbles > 0)
        {
            Debug.Log("LOAD");
            LoadMarble();
        }
    }
    public void ShootMarble()
    {
        marbleHolder.DOKill();
        marbleHolder.DOLocalMove(restPosition, releaseSpeed).SetEase(Ease.OutElastic);
        
        StartCoroutine(waitToRelease());
    }

    private void LoadMarble()
    {
        marbleLocked = true;
        currentMarble = Instantiate(marblePrefab, marbleHolder.position, marbleHolder.rotation);
        currentMarble.SetColor(Random.ColorHSV(0f, 1f, 0f, 0.9f, 1f, 1f));
    }
    private IEnumerator waitToRelease()
    {
        Vector3 releasePos = marbleHolder.localPosition;
        float distToRest = (releasePos - restPosition).magnitude;
        
        yield return new WaitForSeconds(releaseTiming * releaseSpeed);
        remainingMarbles--;
        marbleLocked = false;
        Vector3 throwDirection = marbleHolder.forward;
        float intensity = (distToRest / (releaseSpeed *releaseTiming))*throwIntensity;
        currentMarble.Shoot(throwDirection, intensity);
        if (remainingMarbles <= 0)
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
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
