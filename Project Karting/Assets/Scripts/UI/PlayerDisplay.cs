using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinText;
    [SerializeField] private string joinMessage;
    [SerializeField] private string addPlayerMessage;
    [SerializeField] public Transform displayParent;

    public void ShowJoinMessage()
    {
        joinText.enabled = true;
        joinText.text = joinMessage;
    }
    public void HideMessage()
    {
        joinText.enabled = false;
    }
    public void ShowAddPlayerMessage()
    {
        joinText.enabled = true;
        joinText.text = addPlayerMessage;
    }
}
