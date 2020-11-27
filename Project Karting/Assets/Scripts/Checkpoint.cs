using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kart;
public class Checkpoint : MonoBehaviour
{
    public int checkpointId;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Checkpoint touché");
        KartCollider kart = other.gameObject.GetComponent<KartCollider>();
        if (kart != null)
        {
            Debug.Log("kart pas null");
            GameManager.Instance.checkpointPassed(checkpointId,kart.kartBase.raceInfo.playerId);
        }
    }
}
