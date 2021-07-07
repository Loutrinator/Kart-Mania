using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public Button firstButton;

    public void SelectFirstButton()
    {
        firstButton.Select();
    }
}
