using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemWheelSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    private float rotation = 200f; 
    private float resetAngleOffset = 45f;
    private bool updated = false;
    private void Update()
    {
        if (!updated)
        {
            //Debug.Log("transform.eulerAngles.z " + transform.eulerAngles.z);
            if (transform.rotation.eulerAngles.z > rotation)
            {
                updated = true;
                image.enabled = !image.enabled;
            }  
        }else if(transform.eulerAngles.z < resetAngleOffset)
        {
            updated = false;
        }
    }
}
