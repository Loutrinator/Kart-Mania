using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contains parameters that can adjust the kart's behaviors temporarily.
/// </summary>
public class StatPowerup
{
    public Stats modifiers;
    [HideInInspector] public float elapsedTime;
    public float maxTime;
    
    public Action powerupUsed;

    public StatPowerup(Stats modif, float time)
    {
        modifiers = modif;
        maxTime = time;
        elapsedTime = 0;
    }
}