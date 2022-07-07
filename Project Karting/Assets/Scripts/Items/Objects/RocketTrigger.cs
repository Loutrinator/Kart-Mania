using System;
using Kart;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Items {
    public class RocketTrigger : MonoBehaviour {
        [NonSerialized] public KartBase player;
        
        private void OnTriggerEnter(Collider other) {
            var kart = other.GetComponentInParent<KartBase>();
            if (kart != null && player != null && kart != player) {
                kart.Damaged();
                Destroy(gameObject);
            }
        }
        
    }
}