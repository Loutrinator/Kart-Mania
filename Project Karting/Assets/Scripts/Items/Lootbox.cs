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
            StartCoroutine(WaitToRespawn());
            prism.localScale = Vector3.zero;
        }

        private void GiveItem(int position, KartBase kart)
        {
            Debug.Log("GIVE ITEM TO " + position);
            ItemTomVersion itemTomVersion = GameManager.Instance.itemManager.GetRandomItem(position);
            if (itemTomVersion != null)
            {
                Debug.Log("ITEM : " + itemTomVersion.name);
            }
            else
            {
                Debug.LogError("NO ITEM");
            }
        }

        IEnumerator WaitToRespawn()
        {
            Debug.Log("WAIT");
            yield return new WaitForSeconds(timeToRespawn);
            Debug.Log("RESPAWN");
            state = LootBoxState.fadeIn;
            fadeInStartTime = Time.time;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("TriggerEnter");
            if (state == LootBoxState.available)
            {
                Debug.Log("Available");
                KartBase kart = other.GetComponent<KartCollider>().kartBase;
                if (kart != null)
                {
                    Debug.Log("Kart not null");
                    BreakLootBox(1,kart);
                }
            }
        }
    }
}
