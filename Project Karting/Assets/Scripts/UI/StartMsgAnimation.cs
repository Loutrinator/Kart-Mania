using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Handlers;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class StartMsgAnimation : MonoBehaviour {
        public Action placeKeysAction;

        [SerializeField] private List<Sprite> icons;
        [SerializeField] private float timeBeforeCountDown = 2f;
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float startSFX = 0.2f;
        private Image _image; //image affichant le sprite
        private AudioSource _audioSource;
        private bool countDown;

        private void Awake() {
            _image = GetComponent<Image>();
            _audioSource = GetComponent<AudioSource>();

            _image.transform.localScale = Vector3.zero;
            _image.transform.rotation = Quaternion.Euler(0, 0, -30);
            //delay
            StartCoroutine(WaitToStartCountDown());
        }

        private IEnumerator WaitToStartCountDown() {
            yield return new WaitForSeconds(timeBeforeCountDown - 1f);
            placeKeysAction.Invoke();
            yield return new WaitForSeconds(1f);
            countDown = true;
            yield return AnimateNumber(icons[0]);
            yield return AnimateNumber(icons[1]);
            yield return AnimateNumber(icons[2]);
            yield return AnimateGo(icons[3]);
            if (!RaceManager.Instance.RaceHadBegun()) GameManager.Instance.StartRace();
        }

        private IEnumerator AnimateNumber(Sprite sprite) {
            _image.sprite = sprite;
            Color color = _image.color;
            color.a = 1f;
            _image.color = color;
            _image.transform.localScale = Vector3.zero;
            _image.transform.rotation = Quaternion.Euler(0, 0, -30);
            _image.transform.DOScale(1f, 1f).SetEase(Ease.OutElastic);
            _image.transform.DORotate(new Vector3(0, 0, 1), 1f).SetEase(Ease.OutElastic);
            _image.DOFade(0f, 1f).SetEase(Ease.InExpo);
            yield return new WaitForSeconds(startSFX);
            _audioSource.Play();
            yield return new WaitForSeconds(fadeInDuration - startSFX);
        }

        private IEnumerator AnimateGo(Sprite sprite) {
            _image.sprite = sprite;
            Color color = _image.color;
            color.a = 1f;
            _image.color = color;
            _image.transform.localScale = Vector3.one * 6f;
            _image.transform.DOScale(2f, 0.25f).SetEase(Ease.OutBack);
            _image.DOFade(0f, 2f).SetEase(Ease.InCirc);
            _audioSource.pitch = 1.65f;
            yield return new WaitForSeconds(startSFX);
            _audioSource.Play();
        }
    }
}