using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] private Image image;
    public void SetValue(float value)
    {
        image.fillAmount = value;
    }
}
