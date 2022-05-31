using UnityEngine;
using UnityEngine.EventSystems;

namespace MainMenu {
    public class GoCanvasManager : MonoBehaviour
    {
        public Animator doorAnimator;
        public Animator canvasAnimator;
        public void OpenDoor()
        {
            SoundManager.Instance.PlayUIClick();
            doorAnimator.SetTrigger("open");
            canvasAnimator.SetBool("visible", false);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}