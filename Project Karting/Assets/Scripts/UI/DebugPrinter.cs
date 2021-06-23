using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPrinter : MonoBehaviour
{
    [SerializeField] private Text debugText;
    
    public static DebugPrinter Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void changeColor(Color color)
    {
        debugText.color = color;
    }

    public void print(string value)
    {
        debugText.text = value;
    }
}
