using System;
using DG.Tweening;
using UnityEngine;
using UnityExtendedEditor.Attributes;

namespace UI {
    public class ScreenEffects : MonoBehaviour {
        [SerializeField] private CanvasGroup blackScreen;
        private static ScreenEffects Instance { get; set; }

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
            }
            Instance = this;
            Instance.blackScreen.alpha = 0;
        }

        [Button]
        public static void BlackFade(Action onBlackScreen = null) {
            Instance.blackScreen.DOFade(1, 0.5f)
                .OnComplete(() => {
                    onBlackScreen?.Invoke();
                    Instance.blackScreen.DOFade(0, 0.5f).SetEase(Ease.Linear);
                }).SetEase(Ease.Linear);
        }
    }
}