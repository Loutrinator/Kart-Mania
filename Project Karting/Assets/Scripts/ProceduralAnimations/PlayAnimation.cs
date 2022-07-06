using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ProceduralAnimations {
    public class PlayAnimation : MonoBehaviour {
        [SerializeField] private ProceduralAnimationData animationData;
        [SerializeField] private Transform objectTransform;
        [SerializeField] private string transitionKey;
        [SerializeField] private bool playReverse;
        [SerializeField] private GameObject onCompleteObjectToSelect;
        [SerializeField] private UnityEvent onComplete;

        public void Invoke() {
            EventSystem.current.SetSelectedGameObject(null);
            if (playReverse) {
                animationData.PlayTransitionReverse(objectTransform, transitionKey, () => {
                    onComplete?.Invoke();
                    SelectUIElement();
                });
            }
            else {
                animationData.PlayTransition(objectTransform, transitionKey, () => {
                    onComplete?.Invoke();
                    SelectUIElement();
                });
            }
        }

        public void SelectUIElement() {
            EventSystem.current.SetSelectedGameObject(onCompleteObjectToSelect);
        }
    }
}