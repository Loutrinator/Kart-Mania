using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlTypeDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerNameDisplay;
    [SerializeField]
    private Image typeDisplay;
    [SerializeField]
    private Image ready;
    
    private ControlType controlType;

    private void Awake()
    {
        ready.enabled = false;
    }

    public void SetupUI(int id, ControlType c)
    {
        controlType = c;
        playerNameDisplay.text = "P" + (id + 1);
        Color color = UISettings.instance.colors[id];
        color.a = 1;
        typeDisplay.color = color;
        playerNameDisplay.color = color;
        typeDisplay.sprite = UISettings.instance.controllerTypeImages[(int) c];
        
    }

    public void PlayerIsReady()
    {
        ready.enabled = true;
    }
}

public enum ControlType
{
    OTHER, KEYBOARD, PS, XBOX
}