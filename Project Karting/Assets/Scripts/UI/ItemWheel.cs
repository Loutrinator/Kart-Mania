using System.Collections.Generic;
using DG.Tweening;
using Items;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI {
    public class ItemWheel : MonoBehaviour
    {
        [Header("Wheel")]
        [SerializeField] private RectTransform movingParts;
        [SerializeField] private Image currentItemImage;
        [SerializeField] private RectTransform currentItemTransform;
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
        [Header("Sprites")]
        [SerializeField] private List<ItemWheelSlot> slots;
    
        [Header("Audio")]
        [SerializeField] private AudioSource clickSource;
        [SerializeField] private AudioSource obtainedItemSource;
        [SerializeField] private List<AudioClip> clickSounds;
        private float elapsedBetweenClicks;
        private bool playClicks;
    

        private float rotationSpeed = 0f;
        private float rotationCounter = 0f;

        private PlayerRaceInfo kart;
        private void Start()
        {
            movingParts.localScale = Vector3.one * minScale;
            Vector3 arrowPos = arrow.localPosition;
            arrowPos.x = maxArrowX;
            maxArrowPos = arrowPos;
            arrowPos.x = minArrowX;
            minArrowPos = arrowPos;
        
            arrow.localPosition = minArrowPos;
            currentItemTransform.localScale = Vector3.zero;
        }

        private void Update()
        {
            Vector3 movingPartsRotation = movingParts.rotation.eulerAngles;
            movingPartsRotation.z += rotationSpeed * Time.deltaTime;
            rotationCounter += rotationSpeed * Time.deltaTime;
            movingParts.rotation = Quaternion.Euler(movingPartsRotation);
            if (rotationCounter > 360f)
            {
                rotationCounter -= 360f;
            }

            if (playClicks)
            {
                elapsedBetweenClicks += Time.deltaTime;
                if (rotationSpeed > 0)
                {
                    float timeBeforeClick = 45f / rotationSpeed;
                    if (elapsedBetweenClicks >= timeBeforeClick)
                    {
                        //elapsedBetweenClicks -= timeBeforeClick;
                        elapsedBetweenClicks = 0;
                        AudioClip clickSound = clickSounds[Random.Range(0,clickSounds.Count)];
                        clickSource.clip = clickSound;
                        clickSource.Play();
                    }
                }
            }
        
        }

        /// <summary>
        /// Starts the complete animation of the wheel and picks an item
        /// </summary>
        public void StartSelection(PlayerRaceInfo kartInfo)
        {
            kart = kartInfo;
            ShowWheel();
        }

        private void ShowWheel()
        {
            ResetWheel();
            arrow.DOLocalMove(maxArrowPos, zoomInLength).SetEase(Ease.OutBack);
            movingParts.DOScale(maxScale, zoomInLength).SetEase(Ease.OutBack).OnComplete(Spin);
        }

        private void ResetWheel()
        {
            movingParts.rotation = Quaternion.Euler(Vector3.zero);
            foreach (var slot in slots)
            {
                slot.Reset();
            }
        }

        private void HideWheel()
        {
            arrow.DOLocalMove(minArrowPos, zoomInLength).SetEase(Ease.InBack);
            movingParts.DOScale(minScale, zoomInLength).SetEase(Ease.InBack);
        }

        private void Spin()
        {
            playClicks = true;
            elapsedBetweenClicks = 0;
            DOTween.To(() => rotationSpeed, x => rotationSpeed = x, spinSpeed, 0.5f).SetEase(Ease.OutCubic).OnComplete(
                () => DOTween.To(() => rotationSpeed, x => rotationSpeed = x, 0, 2.5f).SetEase(Ease.InOutSine).OnComplete(PickItem));
        }

        private void PickItem()
        {
            obtainedItemSource.Play();
            playClicks = false;
            currentItemTransform.DOScale(1, 0.2f).SetEase(Ease.OutBack).OnComplete(HideWheel);
        
            foreach (var slot in slots)
            {
                if (slot.IsPointedByArrow(8, 45f))
                {
                    ItemData item = slot.GetItem();
                    Sprite sprite = item.GetIcon();
                    currentItemImage.sprite = sprite;
                    kart.Item = item.GiveItem(kart.kart.transform);
                    Debug.Log("foundSlot "+ sprite.name);
                    return;
                }
            }
            Debug.LogError("PAS DE SLOT CHOISIS");
        }
        public void UseItem()
        {
            currentItemTransform.DOKill();
            currentItemTransform.DOScale(0, 0.2f).SetEase(Ease.InBack);
        }
    }
}
