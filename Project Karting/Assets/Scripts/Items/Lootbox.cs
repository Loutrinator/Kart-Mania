using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootbox : MonoBehaviour
{
    public enum LootBoxState {available,opened,fadeIn}
    public bool breakLoot;
    public float timeToRespawn = 5f;
    public Transform prism;
    public AnimationCurve fadeInSize;
    public float fadeInDuration = 1f;

    private LootBoxState state;
    private float fadeInStartTime;
    private void Update()
    {
        if (breakLoot)
        {
            breakLoot = false;
            BreakLootBox(1);
        }

        if (state == LootBoxState.fadeIn){
            float elapsed = Time.time - fadeInStartTime;
            float scale = fadeInSize.Evaluate(elapsed / fadeInDuration);
            prism.localScale = Vector3.one*scale;
            
            if (elapsed >= timeToRespawn)
            {
                state = LootBoxState.available;
            }
        }
    }

    private void BreakLootBox(int position)
    {
        state = LootBoxState.opened;
        StartCoroutine(WaitToRespawn());
        prism.localScale = Vector3.zero;
    }

    IEnumerator WaitToRespawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        state = LootBoxState.fadeIn;
        fadeInStartTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == LootBoxState.available)
        {
            BreakLootBox(1);
        }
    }
}
