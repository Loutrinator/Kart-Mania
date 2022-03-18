using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionDebugger : MonoBehaviour
{
    private struct ActionDebugColors
    {
        public static Color green = new Color(0.6f, 0.941f, 0.290f, 1f);
        public static Color red = new Color(0.941f, 0.341f, 0.294f, 1f);
    }
    
    
    [SerializeField] private Text nameText;
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    

    public void setValues(float value, bool selected)
    {
        slider.value = value;
        image.color = selected ? ActionDebugColors.green : ActionDebugColors.red;
    }
    public void setName(string name)
    {
        nameText.text = name;
    }
}
