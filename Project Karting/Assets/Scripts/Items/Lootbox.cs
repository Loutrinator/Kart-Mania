using System;
using System.Collections;
using System.Collections.Generic;
using Handlers;
using Kart;
using UnityEngine;

namespace Items
{
    public class Lootbox : MonoBehaviour
    {
        public enum LootBoxState {available,opened,fadeIn}
        public float timeToRespawn = 5f;
        public Transform prism;
        public AnimationCurve fadeInSize;
        public float fadeInDuration = 1f;

        public LootBoxState state;
        private float fadeInStartTime;
        private void Update()
        {
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

        private void BreakLootBox(int position,KartBase kart)
        {
            state = LootBoxState.opened;
            GiveItem(position, kart);
            StartCoroutine(WaitToRespawn());
            prism.localScale = Vector3.zero;
        }

        private void GiveItem(int position, KartBase kart)
        {
            ItemData item = RaceManager.Instance.itemManager.GetRandomItem(position);
            if (item != null)
            {
                RaceManager.Instance.GetPlayerRaceInfo(kart.GetPlayerID()).Item = item.GiveItem(kart.transform);
            }
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
                KartBase kart = other.GetComponentInParent<KartCollisions>().kartBase;
                if (kart != null)
                {
                    BreakLootBox(1,kart);
                }
            }
        }
    }
}
