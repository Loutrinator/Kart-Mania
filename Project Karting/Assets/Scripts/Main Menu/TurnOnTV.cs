using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnTV : MonoBehaviour
{
    public Animator TV;
    public void SwitchTV()
    {
        TV.SetTrigger("switch");
    }
}
