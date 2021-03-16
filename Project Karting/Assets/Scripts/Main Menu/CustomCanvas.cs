using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCanvas : MonoBehaviour
{
    public List<Button> buttons;
    public void enableUIInteraction()
    {
        foreach (var button in buttons)
        {
            button.enabled = true;
        }
    }

    public void disableUIInteraction()
    {
        foreach (var button in buttons)
        {
            button.enabled = false;
        }
    }
}
