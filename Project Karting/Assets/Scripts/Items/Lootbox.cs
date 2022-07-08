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
            PlayerRaceInfo infos = RaceManager.Instance.GetPlayerRaceInfo(kart.GetPlayerID());
            if (!infos.hasItem)
            {
                GiveItem(position, infos);
            }
            StartCoroutine(WaitToRespawn());
            prism.localScale = Vector3.zero;
        }

        private void GiveItem(int position, PlayerRaceInfo info)
        {
            if (info.kart.itemWheel != null)
            {
                info.kart.itemWheel.StartSelection(info);
            }else
            {
                StartCoroutine(giveItemNoUI(position, info));
            }
        }

        private IEnumerator giveItemNoUI(int position, PlayerRaceInfo infos)
        {
            ItemData item = RaceManager.Instance.itemManager.GetRandomItem(position);
            if (item != null)
            {
                yield return new WaitForSeconds(3f);
                infos.Item = item.GiveItem(infos.kart.transform);
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
            if (state == LootBoxState.available) {
                var kart = other.GetComponentInParent<KartCollisions>();
                if (kart == null) return;
                var kartBase = kart.kartBase;
                if (kartBase != null)
                {
                    BreakLootBox(1,kartBase);
                }
            }
        }
    }
}
