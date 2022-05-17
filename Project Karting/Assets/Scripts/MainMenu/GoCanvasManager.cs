using UnityEngine;
using UnityEngine.EventSystems;

public class GoCanvasManager : MonoBehaviour
{
    public Animator doorAnimator;
    public Animator canvasAnimator;
    public void OpenDoor()
    {
        SoundManager.Instance.PlayUIClick();
        doorAnimator.SetTrigger("open");
        canvasAnimator.SetBool("visible", false);
        FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
    }
}