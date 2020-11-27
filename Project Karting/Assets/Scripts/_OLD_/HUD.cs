using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Vehicle vehicle;


    public Text InfoText;
    public Image BrakeImage;
    void FixedUpdate()
    {
        InfoText.text = (int) vehicle.Speed + "km/h\n"
                                            + "Gear " + vehicle.motor.CurrentGear + "\n"
                                            + "wheels " + (int) vehicle.motor.Rpm + "t/min\n"
                                            + "wheels " +
                                            (int) vehicle.motor.Rpm * vehicle.wheelColliders[0].radius *
                                            vehicle.wheelColliders[0].radius * Mathf.PI/1000*60  + "km/h\n";
        BrakeImage.color = vehicle.isBracking ? Color.red : Color.white;
    }
}
