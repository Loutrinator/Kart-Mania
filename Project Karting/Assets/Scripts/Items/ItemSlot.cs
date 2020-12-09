using System;
using UnityEngine;
using Kart;
using UnityEngine.Serialization;

namespace Items
{
    public class ItemSlot : MonoBehaviour
    {
        public KartBase owner;
        [FormerlySerializedAs("currentItem")] public ItemAntoineVersion currentItemAntoineVersion;

        private void Start()
        {
            currentItemAntoineVersion.owner = owner;
        }

        private void Update()
        {
            if (Input.GetKeyDown("i"))
            {
                currentItemAntoineVersion.Use();
            }
        }
    }
}