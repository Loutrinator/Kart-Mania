using System;
using System.Collections;
using System.Collections.Generic;
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

        private LootBoxState state;
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
            StartCoroutine(WaitToRespawn());
            prism.localScale = Vector3.zero;
            GiveItem(position, kart);
        }

        private void GiveItem(int position, KartBase kart)
        {
            Item item = GameManager.Instance.itemManager.GetRandomItem(position);
            Debug.Log("ITEM : " + item.name);
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
                KartBase kart = other.GetComponent<KartBase>();
                if (kart != null)
                {
                    BreakLootBox(1,kart);
                }
            }
        }
    }
}
