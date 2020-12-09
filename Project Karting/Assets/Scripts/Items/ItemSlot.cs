using System;
using UnityEngine;
using Kart;

namespace Items
{
    public class ItemSlot : MonoBehaviour
    {
        public KartBase owner;
        public Item currentItem;

        private void Start()
        {
            currentItem.owner = owner;
        }

        private void Update()
        {
            if (Input.GetKeyDown("i"))
            {
                currentItem.Use();
            }
        }
    }
}