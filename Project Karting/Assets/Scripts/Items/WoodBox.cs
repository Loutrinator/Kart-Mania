using System;
using Kart;
using UnityEngine;

namespace Items
{
    public class WoodBox : MonoBehaviour
    {
        public void Throw(bool isHoldingKey)
        {
            if (!isHoldingKey) transform.SetParent(null);
        } 
        
        //TODO : on trigger enter affect kart 

        private void OnTriggerEnter(Collider other)
        {
            KartBase kart = other.GetComponent<KartCollider>()?.kartBase;
            if (kart != null)
            {
                //TODO : trigger kart crash animation
                Destroy(gameObject);
            }
        }
    }
}