using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemWheel : MonoBehaviour
{
    [Header("Wheel")]
    [SerializeField] private RectTransform movingParts;
    [SerializeField] private float minScale = 0.23f;
    [SerializeField] private float maxScale = 1;

    [Header("Arrow")]
    [SerializeField] private RectTransform arrow;
    [SerializeField] private float minArrowX = 80;
    [SerializeField] private float maxArrowX = 360;
    private Vector3 minArrowPos;
    private Vector3 maxArrowPos;
    
    [Header("Animation")]
    [SerializeField] private float zoomInLength = 0.5f;
    [SerializeField] private float zoomOutLength = 0.8f;
    [SerializeField] private float spinLength = 3f;
    [SerializeField] private float spinSpeed = 3f;

    private float rotationSpeed = 0f;
    private void Start()
    {
        movingParts.localScale = Vector3.one * minScale;
        Vector3 arrowPos = arrow.localPosition;
        arrowPos.x = maxArrowX;
        maxArrowPos = arrowPos;
        arrowPos.x = minArrowX;
        minArrowPos = arrowPos;
        
        arrow.localPosition = minArrowPos;
        StartCoroutine(Animate());
    }

    private void Update()
    {
        Vector3 movingPartsRotation = movingParts.rotation.eulerAngles;
        Debug.Log("rotationSpeed " + rotationSpeed);
        movingPartsRotation.z += rotationSpeed * Time.deltaTime;
        movingParts.rotation = Quaternion.Euler(movingPartsRotation);
    }

    IEnumerator Animate()
    {
        WaitForSeconds wait1s = new WaitForSeconds(1f);
        int count = 0;
        while (count < 100)
        {
            yield return wait1s;
            arrow.DOLocalMove(maxArrowPos, zoomInLength).SetEase(Ease.OutBack);
            movingParts.DOScale(maxScale, zoomInLength).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(zoomInLength);
            DOTween.To(() => rotationSpeed, x => rotationSpeed = x, spinSpeed, 0.5f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(spinLength-1f);
            DOTween.To(() => rotationSpeed, x => rotationSpeed = x, 0, 1.0f).SetEase(Ease.InOutCubic);
            yield return wait1s;
            Debug.Log("Item picked !");
            yield return new WaitForSeconds(3f);
            arrow.DOLocalMove(minArrowPos, zoomInLength).SetEase(Ease.InBack);
            movingParts.DOScale(minScale, zoomInLength).SetEase(Ease.InBack);
            yield return wait1s;
            movingParts.DOKill();
            arrow.DOKill();
            count++;
        }
    }
}
