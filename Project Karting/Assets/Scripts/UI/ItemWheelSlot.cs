using System;
using System.Collections;
using System.Collections.Generic;
using Handlers;
using Items;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ItemWheelSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    public float rotationz = 0; 
    private float rotation = 200f; 
    private float resetAngleOffset = 45f;
    private bool updated = false;
    private bool hidden = false;
    private ItemData item;

    private void Start()
    {
        UpdateItem();
    }

    private void Update()
    {
        rotationz = transform.rotation.eulerAngles.z;
        if (!updated)
        {
            //Debug.Log("transform.eulerAngles.z " + transform.eulerAngles.z);
            if (transform.rotation.eulerAngles.z > rotation)
            {
                updated = true;
                UpdateItem();
            } 
        }else if(transform.eulerAngles.z < resetAngleOffset)
        {
            updated = false;
        }

        if (!hidden)
        {
            if ((transform.eulerAngles.z+180)%360 > 350)
            {
                hidden = true;
                image.enabled = false;
            }
        }else if((transform.eulerAngles.z+90)%360 < 45)
        {
            hidden = false;
            image.enabled = true;
        }
    }

    public bool isPointedByArrow(int numberOfSubdivisions, float arrowAngle)
    {
        float angleOffset = 360f / (numberOfSubdivisions * 2f);
        float angle = (transform.rotation.eulerAngles.z + 360)%360;
        float borneMin = (arrowAngle - angleOffset + 360) % 360;
        float borneMax = (arrowAngle + angleOffset + 360) % 360;
        return angle >= borneMin && angle < borneMax;
    }
    
    public ItemData GetItem()
    {
        return item;
    }
    private void UpdateItem()
    {
        item = RaceManager.Instance.itemManager.GetRandomItem(0);
        image.sprite = item.GetIcon();
    }

    public void Reset()
    {
        hidden = false;
        image.enabled = true;
    }
}
