using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diaphragm : MonoBehaviour
{
    public List<Animator> blades;
    public Transform currentKart;
    public float kartYAxisExitDetection = -2.5f;
    public float kartYAxisDeleteDetection = -4f;
    
    private bool currentlyOpen;

    private void Update()
    {
        if (currentKart != null)
        {
            
            if (currentlyOpen && currentKart.position.y <= kartYAxisExitDetection)//(!open && currentlyOpen)
            {
                Close();
            }
            if (!currentlyOpen && currentKart.position.y <= kartYAxisDeleteDetection)//(!open && currentlyOpen)
            {
                Destroy(currentKart.gameObject);
                currentKart = null;
            }
        } 
    }

    public void Open()
    {
        currentlyOpen = true;
        foreach (var blade in blades)
        {
            blade.SetBool("open",true);
        }
    }
    private void Close()
    {
        currentlyOpen = false;
        foreach (var blade in blades)
        {
            blade.SetBool("open",false);
        }
    }
}
