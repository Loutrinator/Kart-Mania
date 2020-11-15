using System.Collections;
using System.Collections.Generic;
using Kart;
using UnityEngine;


/// <summary>
/// Contains parameters that can adjust the kart's behaviors temporarily.
/// </summary>
[System.Serializable]
public class StatPowerup
{
    public Stats modifiers;
    public float elapsedTime;
    public float maxTime;
}