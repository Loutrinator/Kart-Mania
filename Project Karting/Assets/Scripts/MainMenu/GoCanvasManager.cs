using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GoCanvasManager : MonoBehaviour
{
    public Animator doorAnimator;
    public Animator canvasAnimator;
    public void OpenDoor()
    {
        SoundManager.Instance.PlayUIClick();
        doorAnimator.SetTrigger("open");
        canvasAnimator.SetBool("visible", false);
    }
}