using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTypeDisplayCanvas : MonoBehaviour
{
    void Start()
    {
        PlayerConfigurationManager.Instance.ControlTypeDisplayParent = gameObject;
    }
}
