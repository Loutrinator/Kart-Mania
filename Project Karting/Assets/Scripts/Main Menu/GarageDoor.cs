using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    public MenuManager menuManager;
    
    public void DoorIsOpen()
    {
        menuManager.ShowTransition();
    }
}
